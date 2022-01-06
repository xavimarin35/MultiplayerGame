using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankSpawner : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void SpawnTank(int playerNumber)
    {
        GameObject sp1 = GameObject.Find("Spawn1");
        GameObject sp2 = GameObject.Find("Spawn2");
        GameObject sp3 = GameObject.Find("Spawn3");
        GameObject sp4 = GameObject.Find("Spawn4");

        switch (playerNumber)
        {
            case 1:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/TankBlue", sp1.transform.position, sp1.transform.rotation);
                break;
            case 2:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/TankRed", sp2.transform.position, sp2.transform.rotation);
                break;
            case 3:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/TankYellow", sp3.transform.position, sp3.transform.rotation);
                break;
            case 4:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/TankGreen", sp4.transform.position, sp4.transform.rotation);
                break;
        }

        Destroy(this.gameObject);
    }
}
