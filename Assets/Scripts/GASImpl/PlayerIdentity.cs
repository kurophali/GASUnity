using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerIdentity : NetworkBehaviour
{
    public static int playerCount { get; private set; } = 0;
    public static List<PlayerIdentity> playerIdentities { get; private set; } = new List<PlayerIdentity>();
    
    [SerializeField] GameObject mGunnerPrefab;
    GameObject mGunnerInstance;

    GameObject factionDisplay;

    public NetworkConnection playerConnection { get; private set; } 
    public int playerFaction { get; private set; } = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (isServer) 
        {
            playerCount += 1;
            playerIdentities.Add(this);
            playerConnection = this.GetComponent<NetworkIdentity>().connectionToClient;
            playerFaction = playerCount;
            RpcSetFaction(playerFaction);
            Debug.Log("playerCount : " + playerCount);
        };

        if (!isLocalPlayer) return;
        CmdSpawnGunner(this.gameObject);

        factionDisplay = GameObject.Find("PlayerFaction");
        factionDisplay.GetComponent<Text>().text = "Player faction : " + playerFaction.ToString();
    }

    private void OnDestroy()
    {
        if (isServer) 
        {
            playerCount -= 1;
            playerIdentities.Remove(this);
            Debug.Log("playerCount : " + playerCount);
        }

        Destroy(mGunnerInstance);
    }

    [Command] void CmdSpawnGunner(GameObject spawner)
    {
        if (spawner == null) { Debug.Log("Spawner is null"); return; }

        int spawnerFaction = spawner.GetComponent<PlayerIdentity>().playerFaction;
        mGunnerInstance = Instantiate(mGunnerPrefab);
        IGameplayEntity gameplayEntity = mGunnerInstance.GetComponent<IGameplayEntity>();

        // BUG_HERE : mProperty is not set even in Start()
        if(gameplayEntity == null)
        {
            Debug.Log("Gameplay Entity is null");
        }
        if(gameplayEntity.mProperty == null)
        {
            Debug.Log("Property is null");
        }
        mGunnerInstance.GetComponent<IGameplayEntity>().SetFaction(playerCount);
        NetworkServer.Spawn(mGunnerInstance, spawner);
    }

    [TargetRpc] void RpcSetFaction(int faction)
    {
        playerFaction = faction;
    }
}
