using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] AssetReference mGameSceneReference;
    [SerializeField] Text mDebugText;
    [SerializeField] NetworkManager mNetworkManager;

    public void StartClient()
    {
        mGameSceneReference.LoadSceneAsync().Completed += (handle) =>
        {
            mNetworkManager.StartClient();
        };
        //SceneManager.LoadScene("SampleScene");
        //mNetworkManager.StartClient();
    }

    public void StartServer()
    {
        mGameSceneReference.LoadSceneAsync().Completed += (handle) =>
        {
            mNetworkManager.StartServer();
        };
        //SceneManager.LoadScene("SampleScene");
        //mNetworkManager.StartServer();
    }

    public void GetDownloadSize()
    {
        Addressables.GetDownloadSizeAsync(mGameSceneReference).Completed += (handle) =>
        {
            long size = handle.Result;
            mDebugText.text = size.ToString();
        };
    }
}
