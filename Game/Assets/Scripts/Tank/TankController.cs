using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum TankColor
{
    GREEN,
    BLUE,
    RED,
    YELLOW,

    NONE
}

public class TankController : MonoBehaviourPunCallbacks, IPunObservable
{

    PhotonView PV;
    Rigidbody rb;

    GameObject GM;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();

        if (PV.IsMine)
            Camera.main.GetComponent<FollowCamera>().target = gameObject.transform;
    }

    void Update()
    {
        if (GM == null)
            GM = GameObject.Find("GameManager(Clone)");
    }

    public void OnPhotonSerializeView(PhotonStream stram, PhotonMessageInfo info)
    {

    }
}
