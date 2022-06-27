using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class IGameplayEntity108 : NetworkBehaviour
{
    #region UNITY_CALLBACKS
    void Start()
    {
        if (isServer)
        {

        }
        else
        {
            OnClientNotifySpawnToPlayer();
        }
    }

    #endregion

    #region NOTIFYING_PLAYER
    string mType;
    IPlayerIdentity108 mPlayer;
    int OnClientNotifySpawnToPlayer()
    {
        if (!isServer)
        {
            Debug.Log("Start finding player");
            mPlayer = IPlayerIdentity108.GetPlayer();
            mPlayer.VFOnClientRegisterSpawnedEntity(this);
        }

        return 0;
    }
    #endregion

    #region ADDING_ABILITIES
    List<IGameplayAbility108> mAbilities = new List<IGameplayAbility108>();
    Dictionary<string, int> mAbilityTypeToIdx = new Dictionary<string, int>();

    [Server] public void AddAbility(IGameplayAbility108 abilityInstance)
    {
        
    }
    int mLastRequestedIdx = -1;
    [Command] public void CmdGetAbilityIdx(string abilityTypeName)
    {
        if(mAbilityTypeToIdx.ContainsKey(abilityTypeName))
        {
            RpcReturnAbilityIdx(mAbilityTypeToIdx[abilityTypeName]);
        }
        else
        {
            RpcReturnAbilityIdx(-1);
        }
    }

    [TargetRpc] private void RpcReturnAbilityIdx(int idx)
    {
        mLastRequestedIdx = idx;
    }
    #endregion
}
