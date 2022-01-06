using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;

public class MainManager : MonoBehaviourPunCallbacks
{
    public static MainManager instance;

    public int winner = 0;
    public string winnerStr = " ";

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate("PhotonPrefabs/PlayerManager", Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("PhotonPrefabs/GameManager", Vector3.zero, Quaternion.identity);
        }
        else if (scene.buildIndex == 2)
        {
            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/WinnerManager", Vector3.zero, Quaternion.identity);
        }
    }
}
