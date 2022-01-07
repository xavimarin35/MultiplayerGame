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

    public string winnerStr = ""; // Which tank
    public string winnerName = ""; // Player name

    bool roundstarted = false;
    bool roundwin = false;
    bool win = false;
    bool change_sceen = false;

    public static GameManager Instance;

    public int PlayersRemaining = 0;
    [SerializeField] public bool[] players_alive = { false, false, false, false };

    [SerializeField] bool CountDown = true;
    double StartTime = 0;
    double WinTime = 0;

    GameObject compass;
    GameObject PlayersIcon;
    Text PlayersText;
    GameObject CD_Text;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartTime = PhotonNetwork.Time;
        GameTimer.instance.timerActive = false;

        CD_Text = GameObject.Find("Countdown");

        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        // How many players are there in the room
        PlayersRemaining = PhotonNetwork.PlayerList.Length;

        compass = GameObject.Find("Compass");
        compass.GetComponent<Compass>().player = FindTank(compass.GetComponent<Compass>().actornum);

        for (int i = 0; i < PlayersRemaining; ++i)
        {
            players_alive[i] = true;
        }

        if (!compass.GetComponent<Compass>().allMarkers)
            AssignCompass();
    }

    void Update()
    {
        if (!compass.GetComponent<Compass>().allMarkers)
            AssignCompass();

        double time = PhotonNetwork.Time - StartTime;

        // Countdown Text
        if (CountDown)
        {
            if (time > 1 && time < 2)
            {
                CD_Text.GetComponent<Text>().text = "3";
                //if (!audioplaying)
                //{
                //    audioplaying = true;
                //    CountdownAudioSource.PlayOneShot(CountdownAudioSource.clip);
                //}
            }

            else if (time > 2 && time < 3)
                CD_Text.GetComponent<Text>().text = "2";
            else if (time > 3 && time < 4)
                CD_Text.GetComponent<Text>().text = "1";
            else if (time > 4 && time < 4.5)
                CD_Text.GetComponent<Text>().text = "GO!";
            else if (time > 4.5)
            {
                CountDown = false;
                CD_Text.GetComponent<Text>().enabled = false;
                GameTimer.instance.timeStart = 0;
                GameTimer.instance.timerActive = true;
                //if (!musicplaying)
                //{
                //    musicplaying = true;
                //    Music.PlayOneShot(Music.clip);
                //}
            }
        }

        //if (PlayersText.text != PlayersRemaining.ToString())
        //    PlayersText.text = PlayersRemaining.ToString();

        if (PlayersRemaining == 1 && !win)
        {
            winnerStr = GetWinner();
            winnerName = GetWinner(winnerStr);

            // Show Winner Screen
            GameObject WinUI = GameObject.Find("WinScreen");
            //WinUI.GetComponent<Image>().enabled = true;
            WinUI.GetComponentInChildren<Text>().enabled = true;

            win = true;
            WinTime = PhotonNetwork.Time;
            Debug.Log("Players Remaining: " + PlayersRemaining);
        }

        if (win)
        {
            //GameTimer.instance.timerActive = false;
            GameObject room = GameObject.Find("MainManager");
            room.GetComponent<MainManager>().winnerStr = winnerStr;
            room.GetComponent<MainManager>().winnerName = winnerName;

            if (PhotonNetwork.IsMasterClient /*&& all players accept rematch*/)
            {
                if (PhotonNetwork.Time - WinTime > 1000 && !change_sceen) // 4
                {
                    change_sceen = true;
                    this.photonView.RPC("WinScreen", RpcTarget.All);
                }

            }
        }

        //Debug.Log("Players Remaining: " + PlayersRemaining);
        //Debug.Log("Win: " + win);

    }

    private void AssignCompass()
    {
        GameObject bluemarker = null;
        GameObject redmarker = null;
        GameObject yellowmarker = null;
        GameObject greenmarker = null;

        bluemarker = GameObject.Find("TankBlue(Clone)");
        if (bluemarker != null)
            compass.GetComponent<Compass>().blue = bluemarker.GetComponent<TankMarker>();

        redmarker = GameObject.Find("TankRed(Clone)");
        if (redmarker != null)
            compass.GetComponent<Compass>().red = redmarker.GetComponent<TankMarker>();

        yellowmarker = GameObject.Find("TankYellow(Clone)");
        if (yellowmarker != null)
            compass.GetComponent<Compass>().yellow = yellowmarker.GetComponent<TankMarker>();

        greenmarker = GameObject.Find("TankGreen(Clone)");
        if (greenmarker != null)
            compass.GetComponent<Compass>().green = greenmarker.GetComponent<TankMarker>();

    }

    public string GetWinner()
    {
        string name = " ";
        GameObject lastPlayer = null;

        lastPlayer = GameObject.Find("TankBlue(Clone)");

        if (lastPlayer != null && lastPlayer.activeSelf)
            name = "TankBlue";

        else
        {
            lastPlayer = GameObject.Find("TankRed(Clone)");

            if (lastPlayer != null && lastPlayer.activeSelf)
                name = "TankRed";

            else
            {
                lastPlayer = GameObject.Find("TankYellow(Clone)");

                if (lastPlayer != null && lastPlayer.activeSelf)
                    name = "TankYellow";

                else
                    name = "TankGreen";
            }
        }

        return name;
    }

    public string GetWinner(string tank)
    {
        string name = " ";

        GameObject winner = GameObject.Find(tank + "(Clone)");

        name = winner.GetComponent<PhotonView>().Owner.NickName;

        return name;
    }

    [PunRPC]
    void WinScreen()
    {
        PhotonNetwork.LoadLevel("WinScreen");
    }

    public void OnPlayerDeath(int player_num)
    {
        this.photonView.RequestOwnership();
        PlayersRemaining--;
        players_alive[player_num - 1] = false;
    }

    public int ReturnPlayersLeft()
    {
        return PlayersRemaining;
    }

    public GameObject ReturnPlayerAlive()
    {
        GameObject player = null;
        int actor_num = -1;

        for (int i = 0; i < PlayersRemaining; i++)
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

    public GameObject FindTank(int actor)
    {
        GameObject tank = null;

        switch (actor)
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

    public bool IsPlayerAlive(int actor_number)
    {
        return players_alive[actor_number - 1] == true;
    }

    public bool RoundStarted()
    {
        return roundstarted;
    }

    public bool RoundEnded()
    {
        return roundwin;
    }
    public bool GameStarted()
    {
        return !CountDown;
    }

    public bool GameEnded()
    {
        return win;
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
