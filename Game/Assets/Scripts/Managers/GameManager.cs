﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;

using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Tanks
{
    public class GameManager : MonoBehaviour
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

        public GameObject[] tanks;

        public int tanksSpawned; // Just to check if all tanks are spawning, a counter

        private int m_RoundNumber;
        private WaitForSeconds m_StartWait;
        private WaitForSeconds m_EndWait;
        private TankManager m_RoundWinner;
        private TankManager m_GameWinner;

        public static GameManager Instance;

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Hashtable props = new Hashtable
            {
                {TanksGame.PLAYER_LOADED_LEVEL, true }
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            SpawnAllTanks();

            //SetCameraTargets();

            //StartCoroutine(GameLoop());
        }

        private void SpawnAllTanks()
        {
            PhotonNetwork.Instantiate("Tank", m_TankPrefab.transform.position, m_TankPrefab.transform.rotation, 0);

            // m_Tanks[i].Setup();
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

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(TanksGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                    if ((bool)playerLoadedLevel) continue;

                return false;
            }

            return true;
        }
    }
}
