using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public int scoreOne, scoreTwo;

    public Text textOne, textTwo;

    Vector3 iniPos1, iniPos2;

    private void OnEnable()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        iniPos1.x = -76;
        iniPos2.x = 76;
        iniPos1.y = iniPos2.y = 9; 
        iniPos1.z = iniPos2.z = 77;

        CreatePlayer();
    }

    private void CreatePlayer()
    {
        Debug.Log("Creating Player");

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Paddle"), iniPos1, Quaternion.identity);

        else
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Paddle"), iniPos2, Quaternion.identity);
    }
}
