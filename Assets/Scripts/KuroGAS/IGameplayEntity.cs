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

    [Command] public void CmdAddAbility(int abilityID)
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

    [Command] public void CmdTriggerAbility(int abilityID, Vector3 triggerVector)
    {
        // Trigger the ability if this entity has the ability
        if (abilityID >= mAvailableAbilities.Count 
            || abilityID < 0
            || this.mAvailableAbilities[abilityID] == false) 
            return;

        int triggerResult = mAbilities[abilityID].Trigger(this, triggerVector);
        if (triggerResult != 0) return;

        // Work out different cues for client side
        Vector3 triggerVectorAllies, triggerVectorEnemies;
        triggerVectorAllies = mAbilities[abilityID].VFProcessTriggerVectorForAllies(triggerVector);
        triggerVectorEnemies = mAbilities[abilityID].VFProcessTriggerVectorForEnemies(triggerVector);

        foreach (PlayerIdentity identity in PlayerIdentity.playerIdentities)
        {
            // Trigger after the data are processed on the server, so the client can't cheat
            // In this case, triggerVectorAllies and triggerVectorEnemies are the different data for each faction
            if(identity.playerFaction == mProperty.faction)
            {
                RpcPostTrigger(identity.connectionToClient,
                    abilityID, mAbilities[abilityID].abilityState, triggerVectorAllies);
            }
            else
            {
                RpcPostTrigger(identity.connectionToClient,
                    abilityID, mAbilities[abilityID].abilityState, triggerVectorEnemies);
            }
        }

        return;
    }

    // W.I.P. : This call uses the client's mAbilities so if the value within mAbilities in the client changes
    //          This one will not sync
    [TargetRpc] private void RpcPostTrigger(NetworkConnection conn, int abilityID, int abilityState,
        Vector3 triggerVector)
    {
        mAbilities[abilityID].SetAbilityState(abilityState);
        mAbilities[abilityID].VFOnClientTrigger(triggerVector);
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
        // This part is only on the server's side because
        foreach (IGameplayAbility ability in mAbilityIDsForIteration)
        {
            ability.Update();
        }
    }
}
