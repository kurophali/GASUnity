using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;


// DO NOT MAKE VIRTUAL OR ABSTRACT FUNCTIONS HERE. ONLY EXTENT FOR GAMEPLAY CUE OR PROPERTIES.
[DisallowMultipleComponent] public class IGameplayEntity107 : NetworkBehaviour
{
    #region CALLBACKS || Connection to unity's behaviour callbacks
    protected virtual void Start()
    {
        mNetworkIdentity = GetComponent<NetworkIdentity>();
        InitializeNodeSystem();
        VFInitializeSpawnedObjects();
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

    #region SPAWNED_OBJECTS || Have the owner player add a reference to this object

    IPlayerIdentity107 mPlayer;
    protected virtual void VFInitializeSpawnedObjects()
    {
        if (isServer || !hasAuthority) return;
        mPlayer = IPlayerIdentity107.GetPlayer();
        mPlayer.VFRegisterSpawnedObject(this);;
    }
    #endregion

    #region MODEL_FXNODES || Deals with model nodes 
    // Model nodes are the local transforms attached to the GE prefab. 
    // Use them as where the projectile starts or where particle spawns.
    [SerializeField] protected List<Transform> fxNodes;
    Dictionary<string, int> fxNodeNameToIndex = new Dictionary<string, int>();

    int InitializeNodeSystem()
    {
        for(int i = 0; i < fxNodes.Count; ++i)
        {
            fxNodeNameToIndex[fxNodes[i].gameObject.name] = i;
        }

        return 0;
    }
    public Transform GetNode(int index)
    {
        if (index < 0 || index > fxNodes.Count - 1) return null;
        return fxNodes[index];
    }

    public int FindNodeIndex(string nodeName)
    {
        return fxNodeNameToIndex[nodeName];
    }

    #endregion

    #region ADDING_ABILITY
    public List<bool> mAvailableAbilities { get; private set; } = InitAbilityMask(IGameplayAbility107.gAbilityCount);
    public List<IGameplayAbility107> mAbilities { get; private set; } = InitAbilityList(IGameplayAbility107.gAbilityCount);
    public IGameplayProperty107 mProperty { get; private set; } = new IGameplayProperty107(); // W.I.P. : Doesn't get initialized in Start()
    public NetworkIdentity mNetworkIdentity { get; private set; }
    // This one is used to iterate all abilities and call their UpdateAbility on Update()
    private List<IGameplayAbility107> mAbilityIDsForIteration = new List<IGameplayAbility107>();

    protected virtual void VFInitialize() { }
    protected virtual void VFRelease() { }
    public void SetFaction(int newFaction)
    {
        mProperty.faction = newFaction;
    }

    public void AddAbility(Type abilityType, ref int abilityID)
    {
        abilityID = IGameplayAbility107.gAbilityNameToIndex[abilityType.Name];
        this.CmdAddAbility(abilityID);
    }
    [Command] private void CmdAddAbility(int abilityID)
    {
        int result = IGameplayAbility107.gAbilityDefaults[abilityID].VFOnGEAddAbilityInit(this);
        if (result != 0) return;
        
        if(this.mAbilities == null)
        {
            Debug.Log("mAbilities is null");
        }

        this.mAbilities[abilityID] = (IGameplayAbility107)Activator.CreateInstance(IGameplayAbility107.gAbilityTypes[abilityID]);
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
            result = mAbilities[abilityID].VFOnGERemoveAbilityRelease();
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

        this.mAbilities[abilityID] = (IGameplayAbility107)Activator.CreateInstance(IGameplayAbility107.gAbilityTypes[abilityID]);
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

    #region GAMEPLAY_CUES || Controls transforms and animations of this object on clients in different factions

    private NetworkConnection mServerRpcDestination; // This is used to decided which client to send the cues
    private bool mServerTriggered = false; // true if the server's update has been executed. 
    private int mDstPlayerFaction = -1;
    [Server] void UpdateAbilities()
    {
        // This part is about ability logics so it's only run on the server's side
        foreach (IGameplayAbility107 ability in mAbilityIDsForIteration)
        {
            // use this bool to fire a single trigger on the server
            mServerTriggered = false; 

            // This is the server's local update. After this the mServerTriggered becomes true
            ability.VFOnServerUpdateAbility(this, ability.mTriggerVector);
            mServerTriggered = true;

            // Trigger different behaviours of this object for different factions in the two rpcs down there
            foreach (IPlayerIdentity107 identity in IPlayerIdentity107.gPlayerIdentities)
            {
                mServerRpcDestination = identity.connectionToClient;
                mDstPlayerFaction = identity.playerFaction;

                // The two functions define different cues for allies and enemies
                if (identity.playerFaction == mProperty.faction)
                {
                    ability.VFOnServerUpdateAbilityForAllies(this, ability.mTriggerVector);
                }
                else
                {
                    ability.VFOnServerUpdateAbilityForEnemies(this, ability.mTriggerVector);
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

    // This example is to show how the same skill would trigger different fx on different factions
    [Server]
    public virtual void CueFactionalTranslate(Transform fakeTransform, Vector3 translation)
    {
        Vector3 position = new Vector3(); 
        if (mServerTriggered == false)
        {
            gameObject.transform.position += translation;
            fakeTransform.position -= translation;
        }
        else
        {
            if(mDstPlayerFaction == mProperty.faction)
            {
                position = gameObject.transform.position;
                RpcSyncFactionalTranslate(mServerRpcDestination, position);
            }

        }
    }


    [TargetRpc]
    private void RpcSyncFactionalTranslate(NetworkConnection connectionToClient, Vector3 position)
    {
        gameObject.transform.position = position;
    }
    #endregion

    #region HELPERS
    static List<IGameplayAbility107> InitAbilityList(int size)
    {
        List<IGameplayAbility107> output = new List<IGameplayAbility107>();
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
