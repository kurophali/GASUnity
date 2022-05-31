using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] Text mDebugText;
    //[SerializeField] AssetReference mSceneRef;
    [SerializeField] GameObject mNetworkManager107Prefab;
    [SerializeField] GameObject mNetworkManager108Prefab;
    static GameObject mNetworkManagerInstance;
    NetworkManager mNetworkManager;

    public void StartClient107()
    {
        mNetworkManagerInstance = Instantiate(mNetworkManager107Prefab);
        mNetworkManager = mNetworkManagerInstance.GetComponent<NetworkManager>();

        SceneManager.LoadScene("GAS107");
        mNetworkManager.StartClient();
    }

    public void StartServer107()
    {
        mNetworkManagerInstance = Instantiate(mNetworkManager107Prefab);
        mNetworkManager = mNetworkManagerInstance.GetComponent<NetworkManager>();

        SceneManager.LoadScene("GAS107");
        mNetworkManager.StartServer();
    }
    public void StartClient108()
    {
        mNetworkManagerInstance = Instantiate(mNetworkManager108Prefab);
        mNetworkManager = mNetworkManagerInstance.GetComponent<NetworkManager>();
        SceneManager.LoadScene("GAS108");
        mNetworkManager.StartClient();
    }

    public void StartServer108()
    {
        mNetworkManagerInstance = Instantiate(mNetworkManager108Prefab);
        mNetworkManager = mNetworkManagerInstance.GetComponent<NetworkManager>();
        SceneManager.LoadScene("GAS108");
        mNetworkManager.StartServer();
    }
    //public void GetDownloadSize()
    //{
    //    Addressables.GetDownloadSizeAsync(mGameSceneReference).Completed += (handle) =>
    //    {
    //        long size = handle.Result;
    //        mDebugText.text = size.ToString();
    //    };
    //}
}
