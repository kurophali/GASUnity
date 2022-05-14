using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

// DO NOT MAKE VIRTUAL OR ABSTRACT FUNCTIONS HERE
public class IGameplayEntity : NetworkBehaviour
{
    #region ADDING_ABILITY
    
    public List<bool> mAvailableAbilities { get; private set; }
    public List<IGameplayAbility> mAbilities { get; private set; }
    public IGameplayProperty mProperty { get; private set; } = new IGameplayProperty(); // W.I.P. : Doesn't get initialized in Start()
    public NetworkIdentity mNetworkIdentity { get; private set; }
    private List<IGameplayAbility> mAbilityIDsForIteration;

    protected virtual void VFInitialize() { }
    protected virtual void VFRelease() { }

    private void Start()
    {
        mNetworkIdentity = GetComponent<NetworkIdentity>();

        //mProperty = new IGameplayProperty();

        mAbilities = new List<IGameplayAbility>();
        mAvailableAbilities = new List<bool>();

        // Don't use ability to prevent the client knowing the other object has ability
        mAbilityIDsForIteration = new List<IGameplayAbility>();

        for(int i = 0; i < IGameplayAbility.gAbilityCount; ++i)
        {
            mAbilities.Add(null);
            mAvailableAbilities.Add(false);
        }

        VFInitialize();
    }

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

    private void Update()
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

    [Server] void UpdateAbilities()
    {
        // This part is only on the server's side because the client should not know what abilities have been triggered
        foreach (IGameplayAbility ability in mAbilityIDsForIteration)
        {
            // This is the server's local update.
            ability.VFOnServerUpdateItself(this, ability.mTriggerVectorServer);

            // Trigger different behaviours of this object for different factions in the 2 rpcs down there
            foreach (PlayerIdentity identity in PlayerIdentity.playerIdentities)
            {
                if (identity.playerFaction == mProperty.faction)
                {
                    ability.VFOnServerUpdateAllyRpcs(this, ability.mTriggerVectorAllies);
                }
                else
                {
                    ability.VFOnServerUpdateEnemyRpcs(this, ability.mTriggerVectorAllies);
                }
            }
        }
    }

    #endregion

    #region CONTROLLER_RPCS
    // DEBUG_HERE : Just to group functions here so I don't have to change all 3 of them
    public void CommonTranslate(Vector3 triggerVector)
    {
        if (isServer)
        {
            Translate(triggerVector);
            RpcTranslate(triggerVector);
        }
    }
    public void Translate(Vector3 triggerVector)
    {
        gameObject.transform.position += triggerVector;
    }
    [ClientRpc] public void RpcTranslate(Vector3 translation)
    {
        Translate(translation);
    }
    #endregion
}
