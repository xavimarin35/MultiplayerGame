using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Rematch : MonoBehaviour
{
    public void OnClick()
    {
        int num = PhotonNetwork.LocalPlayer.ActorNumber;
        GameObject obj = GameObject.Find("WinnerManager(Clone)");
        obj.GetComponent<WinManager>().OnClickRematch(num);
    }
}
