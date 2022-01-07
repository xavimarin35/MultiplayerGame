using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Exit : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnExitButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ClosePopUp()
    {
        GameObject.Find("Panel").SetActive(false);
        GameObject.Find("KillerName").SetActive(false);
        GameObject.Find("Spectate").SetActive(false);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Menu");
    }
}
