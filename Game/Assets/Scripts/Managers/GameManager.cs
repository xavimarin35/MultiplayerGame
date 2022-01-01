using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

using Photon.Realtime;


public class GameManager : MonoBehaviourPun, IPunObservable
{
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject m_TankPrefab;     // De moment aquest prefab s'està fent servir només per dir la posició i rotació inicial del tank,
                                        // s'haurà de canviar i determinar diferents punts des d'on poden sortir cada un dels players

    public TankManager[] m_Tanks;       // La llista de TankManager es on es guardaven els tanks abans, ara tenim dos GameObjects que es creen
                                        // i hem de canviar aquesta llista perquè tot el joc està fet en funció del TankManager

    public List<GameObject> tanks;

    public int tanksSpawned; // Just to check if all tanks are spawning, a counter

    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;

    private bool renamedTanks = false;

    public static GameManager Instance;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        // SpawnAllTanks();

        // Nombre de jugadors = Nombre de tanks
        // tanksSpawned = PhotonNetwork.CurrentRoom.PlayerCount;

        //SetCameraTargets();

        //StartCoroutine(GameLoop());
    }

    private void FixedUpdate()
    {
        if (!renamedTanks)
        {
            // AssignTanks(tanksSpawned);

            renamedTanks = true;
        }

    }

    private void SpawnAllTanks()
    {
        PhotonNetwork.Instantiate("PhotonPrefabs/Tank", m_TankPrefab.transform.position, m_TankPrefab.transform.rotation);

        // m_Tanks[i].Setup();
    }

    private void AssignTanks(int tanks)
    {
        for (int i = 0; i < tanks; ++i)
        {
            // If there is a tank named Tank(Clone), change its number and name
            if (GameObject.Find("Tank(Clone)") != null)
            {
                int number = i + 1;

                GameObject.Find("Tank(Clone)").GetComponent<TankMovement>().m_PlayerNumber = number;
                GameObject.Find("Tank(Clone)").name = "Tank" + number;
            }
        }
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(true);
            stream.SendNext(2);
        }
        else
        {

        }
    }
}
