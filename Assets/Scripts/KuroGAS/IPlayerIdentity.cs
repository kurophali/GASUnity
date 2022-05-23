using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class IPlayerIdentity : NetworkBehaviour
{
    public static int gPlayerCount { get; private set; } = 0;
    public static List<IPlayerIdentity> gPlayerIdentities { get; private set; } = new List<IPlayerIdentity>();
    public static IPlayerIdentity GetPlayer()
    {
        foreach(IPlayerIdentity player in gPlayerIdentities)
        {
            if (player.isLocalPlayer)
            {
                return player;
            }
        }

        Debug.LogError("Tried to find player but none was local");
        return null;
    }
    [SerializeField] GameObject mGunnerPrefab;
    GameObject mSpawnedEntities;
    public virtual void VFRegisterSpawnedObject(IGameplayEntity gunner)
    {
        Debug.Log("RegisterSpawnedObject called");
        mSpawnedEntities = gunner.gameObject;
        GetComponent<PlayerLocals>().AssignGameplayEntity(gunner);
    }

    public IGameplayEntity GetGameplayEntity()
    {
        if(mSpawnedEntities == null)
        {
            Debug.LogError("mGunnerInstance is null");
            return null;
        }
        return mSpawnedEntities.GetComponent<IGameplayEntity>();
    }

    GameObject factionDisplay;

    public NetworkConnection playerConnection { get; private set; } 
    public int playerFaction { get; private set; } = 0;
    // Start is called before the first frame update
    void Start()
    {
        gPlayerCount += 1;
        gPlayerIdentities.Add(this);

        if (isServer) 
        {
            playerConnection = this.GetComponent<NetworkIdentity>().connectionToClient;
            playerFaction = gPlayerCount;
            RpcSetFaction(playerFaction);
            Debug.Log("gPlayerCount : " + gPlayerCount);
        };

        if (!isLocalPlayer) return;
        CmdSpawnGameplayEntities(this.gameObject);

        factionDisplay = GameObject.Find("PlayerFaction");
        factionDisplay.GetComponent<Text>().text = "Player faction : " + playerFaction.ToString();
    }

    private void OnDestroy()
    {
        gPlayerCount -= 1;
        gPlayerIdentities.Remove(this);

        if (isServer) 
        {
            Debug.Log("gPlayerCount : " + gPlayerCount);
        }

        Destroy(mSpawnedEntities);
    }

    [Command] void CmdSpawnGameplayEntities(GameObject spawner)
    {
        if (spawner == null) { Debug.Log("Spawner is null"); return; }

        int spawnerFaction = spawner.GetComponent<IPlayerIdentity>().playerFaction;
        mSpawnedEntities = Instantiate(mGunnerPrefab);
        IGameplayEntity gameplayEntity = mSpawnedEntities.GetComponent<IGameplayEntity>();

        // BUG_HERE : mProperty is not set even in Start()
        if(gameplayEntity == null)
        {
            Debug.Log("Gameplay Entity is null");
        }
        if(gameplayEntity.mProperty == null)
        {
            Debug.Log("Property is null");
        }
        mSpawnedEntities.GetComponent<IGameplayEntity>().SetFaction(gPlayerCount);
        NetworkServer.Spawn(mSpawnedEntities, spawner);
    }

    [TargetRpc] void RpcSetFaction(int faction)
    {
        playerFaction = faction;
    }
}
