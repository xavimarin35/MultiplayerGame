using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

using Photon.Realtime;


public class GameManager : MonoBehaviourPun, IPunObservable
{
    // Tot això és el que feiem servir abans, segurament es podrà borrar quan el joc estigui acabat
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject m_TankPrefab;

    public TankManager[] m_Tanks;

    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;

    // Fins aquí ------------------------------------------------------------------------------------

    public static GameManager Instance;

    public int PlayersRemaining = 0;
    [SerializeField] public bool[] players_alive = { false, false, false, false };

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        // How many players are there in the room
        PlayersRemaining = PhotonNetwork.PlayerList.Length;

        for (int i = 0; i < PlayersRemaining; ++i)
        {
            players_alive[i] = true;
        }

        //StartCoroutine(GameLoop());
    }

    // En principi es pot borrar perquè el follow camera ja funciona (WIP)
    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }

    // Tot això s'haurà de borrar o refer perquè el game loop ara és diferent
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_MessageText.text = "";

        while (!OneTankLeft())
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }

    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }

    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    // Fins aquí ---------------------------------------------------------------------------

    public GameObject ReturnPlayerAlive()
    {
        GameObject player = null;
        int actor_num = -1;

        for(int i = 0; i < PlayersRemaining; i++)
        {
            if (players_alive[i] == true)
            {
                actor_num = i + 1;

                player = FindTank(actor_num);

                if (player != null)
                {
                    if (player.GetComponent<PhotonView>().IsMine)
                        return player;
                }
            }
        }

        return player;
    }

    private GameObject FindTank(int actor)
    {
        GameObject tank = null;

        switch(actor)
        {
            case 1:
                tank = GameObject.Find("TankBlue(Clone)");
                break;
            case 2:
                tank = GameObject.Find("TankRed(Clone)");
                break;
            case 3:
                tank = GameObject.Find("TankYellow(Clone)");
                break;
            case 4:
                tank = GameObject.Find("TankGreen(Clone)");
                break;

            default:
                tank = GameObject.Find("TankBlue(Clone)");
                break;
        }

        return tank;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(players_alive);
            stream.SendNext(PlayersRemaining);
        }
        else
        {
            players_alive = (bool[])stream.ReceiveNext();
            PlayersRemaining = (int)stream.ReceiveNext();
        }
    }
}
