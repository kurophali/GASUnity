using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;


// DO NOT MAKE VIRTUAL OR ABSTRACT FUNCTIONS HERE. ONLY EXTENT FOR GAMEPLAY CUE OR PROPERTIES.
[DisallowMultipleComponent] public class IGameplayEntity : NetworkBehaviour
{
    #region CALLBACKS
    protected virtual void Start()
    {
        mNetworkIdentity = GetComponent<NetworkIdentity>();
        VFInitializeOwnership();
        VFInitialize();
    }

    protected virtual void Update()
    {
        if (isServer)
        {
            UpdateAbilities();
        }
    }

    private void OnDestroy()
    {
        VFRelease();
    }
    #endregion

    #region OWNERSHIPS
    IPlayerIdentity mPlayer;
    protected virtual void VFInitializeOwnership()
    {
        if (isServer || !hasAuthority) return;
        mPlayer = IPlayerIdentity.GetPlayer();
        mPlayer.VFRegisterSpawnedObject(this);;
    }
    #endregion

    #region ADDING_ABILITY

    public List<bool> mAvailableAbilities { get; private set; } = InitAbilityMask(IGameplayAbility.gAbilityCount);
    public List<IGameplayAbility> mAbilities { get; private set; } = InitAbilityList(IGameplayAbility.gAbilityCount);
    public IGameplayProperty mProperty { get; private set; } = new IGameplayProperty(); // W.I.P. : Doesn't get initialized in Start()
    public NetworkIdentity mNetworkIdentity { get; private set; }

    private List<IGameplayAbility> mAbilityIDsForIteration = new List<IGameplayAbility>();

    protected virtual void VFInitialize() { }
    protected virtual void VFRelease() { }



    public void SetFaction(int newFaction)
    {
        mProperty.faction = newFaction;
    }

    public void AddAbility(Type abilityType, ref int abilityID)
    {
        abilityID = IGameplayAbility.gAbilityNameToIndex[abilityType.Name];
        this.CmdAddAbility(abilityID);
    }
    [Command] private void CmdAddAbility(int abilityID)
    {
        int result = IGameplayAbility.gAbilityDefaults[abilityID].VFValidateEntityForAbility(this);
        if (result != 0) return;
        
        if(this.mAbilities == null)
        {
            Debug.Log("mAbilities is null");
        }

        this.mAbilities[abilityID] = (IGameplayAbility)Activator.CreateInstance(IGameplayAbility.gAbilityTypes[abilityID]);
        this.mAvailableAbilities[abilityID] = true;
        this.mAbilityIDsForIteration.Add(mAbilities[abilityID]);
        this.mAbilities[abilityID].SetOwner(this);

        RpcAddAbility(this.mNetworkIdentity.connectionToClient, abilityID, result);
    }

    [Command] public void CmdRemoveAbility(int abilityID)
    {
        int result = 0;

        mAbilityIDsForIteration.Remove(mAbilities[abilityID]);

        if (mAbilities[abilityID] != null) 
        {
            result = mAbilities[abilityID].VFRelease();
        }
        
        if (result == 0)
        {
            mAbilities[abilityID] = null;
            mAvailableAbilities[abilityID] = false;
        }

    }

    [TargetRpc] private void RpcAddAbility(NetworkConnection conn, int abilityID, int result)
    {
        if (mAbilities == null) Debug.Log("RpcAddAbility : mAbilities is null");
        if (result != 0) return ;
        if (mAbilities[abilityID] != null) return;
        if (mAvailableAbilities[abilityID] == true) return;

        this.mAbilities[abilityID] = (IGameplayAbility)Activator.CreateInstance(IGameplayAbility.gAbilityTypes[abilityID]);
        this.mAvailableAbilities[abilityID] = true;
        this.mAbilities[abilityID].SetOwner(this);
    }
    #endregion

    #region TRIGGERING_ABILITY
    [Command] public void CmdTriggerAbility(int abilityID, Vector3 triggerVector)
    {
        // Trigger the ability if this entity has the ability
        if (abilityID >= mAvailableAbilities.Count 
            || abilityID < 0
            || this.mAvailableAbilities[abilityID] == false) 
            return;

        // Processing different trigger vectors, setting the mTriggerDetected, etc...
        int triggerResult = mAbilities[abilityID].OnServerTrigger(this, triggerVector);

        return;
    }



    #endregion

    #region GAMEPLAY_CUES

    private NetworkConnection mServerRpcDestination; // This is used to decided which client to send the cues
    private bool mServerTriggered = false; // true if the server's update has been executed. 
    [Server] void UpdateAbilities()
    {
        // This part is about ability logics so it's only run on the server's side
        foreach (IGameplayAbility ability in mAbilityIDsForIteration)
        {
            // use this bool to fire a single trigger on the server
            mServerTriggered = false; 

            // This is the server's local update. After this the mServerTriggered becomes true
            ability.VFOnServerUpdateItself(this, ability.mTriggerVector);
            mServerTriggered = true;

            // Trigger different behaviours of this object for different factions in the two rpcs down there
            foreach (IPlayerIdentity identity in IPlayerIdentity.gPlayerIdentities)
            {
                mServerRpcDestination = identity.connectionToClient;

                // The two functions define different cues for allies and enemies
                if (identity.playerFaction == mProperty.faction)
                {
                    ability.VFOnServerUpdateAllyRpcs(this, ability.mTriggerVector);
                }
                else
                {
                    ability.VFOnServerUpdateEnemyRpcs(this, ability.mTriggerVector);
                }
            }
        }
    }

    // Example : Cue function for the IGameplayAbility class for behaviours
    // Define your every behaviour like this one
    // Expose only one for writing ability cues
    [Server] public virtual void CueTranslate(Vector3 translation)
    {
        Vector3 position = new Vector3(); // If I don't init here the 'else' branch will not go though compilation
        if(mServerTriggered == false)
        {
            // Restricts single run on server
            Translate(translation);
        }
        else
        {
            // Trigger everytime the onServerRpcDestination changes
            // Better set all the values in this branch because usually the client only cares about syncing to the server
            position = gameObject.transform.position;
            RpcSyncTranslate(mServerRpcDestination, position); 
        }
    } 
    [Server] private void Translate(Vector3 triggerVector)
    {
        gameObject.transform.position += triggerVector;
    }
    [TargetRpc] private void RpcSyncTranslate(NetworkConnection connectionToClient, Vector3 position)
    {
        gameObject.transform.position = position;
    }


    // Another example to show you that when writing the server's actual gameplay code,
    // one has to synchronize the minimum cue.
    // But when naming the function one should always know which one causes the result.
    // For instance here we use RpcSyncLookAt instead of RpcSyncRotation
    [Server] public virtual void CueLookAt(Vector3 position)
    {
        // Sync rotations because we dont want other player to know enemy's focus
        Quaternion rotation = new Quaternion(); 
        if (mServerTriggered == false)
        {
            LookAt(position);
        }
        else
        {
           rotation = gameObject.transform.rotation;
           RpcSyncLookAt(mServerRpcDestination, rotation);
        }
    } 
    [Server] private void LookAt(Vector3 position)
    {
        gameObject.transform.LookAt(position);
    }
    [TargetRpc] private void RpcSyncLookAt(NetworkConnection connectionToClient, Quaternion rotation)
    {
        gameObject.transform.rotation = rotation;
    }
    #endregion

    #region HELPERS
    static List<IGameplayAbility> InitAbilityList(int size)
    {
        List<IGameplayAbility> output = new List<IGameplayAbility>();
        for (int i = 0; i < size; ++i) output.Add(null);
        return output;
    }

    static List<bool> InitAbilityMask(int size)
    {
        List<bool> output = new List<bool>();
        for (int i = 0; i < size; ++i) output.Add(false);
        return output;
    }
    #endregion
}
