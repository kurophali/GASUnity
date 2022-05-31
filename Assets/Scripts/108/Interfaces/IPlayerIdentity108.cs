using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class IPlayerIdentity108 : NetworkBehaviour
{
    #region UNITY_CALLBACKS
    private void Start()
    {
        if (isServer)
        {
            OnServerAddPlayer();
            VFOnServerSetFaction();
            RpcNotifyPlayerJoin(mFaction);
            VFOnServerInitializeSpawnables();
        }
        else
        {
            VFOnClientStart();
        }
    }

    private void OnDestroy()
    {
        if (isServer)
        {
            OnServerRemovePlayer(); // Spawned objects are also removed when player is destroyed
            RpcNotifyPlayerLeave();
        }
    }

    private void Update()
    {
        if (isServer)
        {

        }
        else
        {
            VFOnClientUpdate();
        }
    }

    #endregion

    #region SERVER_ADDING_PLAYERS
    public static List<IPlayerIdentity108> gPlayerIdentities { get; private set; } = new List<IPlayerIdentity108>();
    public static int gPlayerCount { get; private set; } = 0;
    public static IPlayerIdentity108 GetPlayer() // This is used on the GameplayEntity to find the master object
    {
        foreach (IPlayerIdentity108 player in gPlayerIdentities)
        {
            if (player.isLocalPlayer)
            {
                return player;
            }
        }

        Debug.LogError("Tried to find player but none was local");
        return null;
    }

    int mFaction;

    private void OnServerAddPlayer()
    {
        gPlayerIdentities.Add(this);
        gPlayerCount = gPlayerIdentities.Count;
        Debug.Log("Player Added to list");
    }

    private void OnServerRemovePlayer()
    {
        gPlayerIdentities.Remove(this);
        gPlayerCount = gPlayerIdentities.Count;
    }

    protected virtual void VFOnServerSetFaction()
    {
        mFaction = gPlayerCount;
    }

    [ClientRpc] void RpcNotifyPlayerJoin(int faction)
    {
        mFaction = faction;
        gPlayerIdentities.Add(this);
        gPlayerCount = gPlayerIdentities.Count;

        Debug.Log("Player notified to client");
    }

    [ClientRpc] void RpcNotifyPlayerLeave()
    {
        gPlayerIdentities.Remove(this);
        gPlayerCount = gPlayerIdentities.Count;
    }
    #endregion

    #region REGISTERING_SPAWNABLES
    protected virtual void VFOnServerInitializeSpawnables() { /* Spawn IGameplayEntity's here*/ }
    public virtual void VFOnClientRegisterSpawnedEntity(IGameplayEntity108 ent) 
    {
        // The entity will call this function after it's spawned on the client
    }

    #endregion

    #region CONTROLLERS
    protected virtual void VFOnClientStart() { }
    protected virtual void VFOnClientUpdate() { }
    #endregion
}
