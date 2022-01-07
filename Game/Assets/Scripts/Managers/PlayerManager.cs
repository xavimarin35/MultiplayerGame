using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameManager GM;

    public bool dead = false;
    public int killer;
    int pos = -1;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {

        if(PV.IsMine)
        {
            CreateController();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GM == null)
            GM = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();
    }

    void CreateController()
    {
        int i = PhotonNetwork.LocalPlayer.ActorNumber;

        GameObject spawn = GameObject.Find("Spawn1");

        switch(i)
        {
            case 1:
                spawn = GameObject.Find("Spawn1");
                break;
            case 2:
                spawn = GameObject.Find("Spawn2");
                break;
            case 3:
                spawn = GameObject.Find("Spawn3");
                break;
            case 4:
                spawn = GameObject.Find("Spawn4");
                break;
        }

        spawn.gameObject.GetComponent<TankSpawner>().SpawnTank(i);
    }
}
