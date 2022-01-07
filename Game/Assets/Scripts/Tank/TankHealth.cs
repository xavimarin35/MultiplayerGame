using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class TankHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    PhotonView PV;
    Scene scene;
    GameObject GM;

    public int killer;
    public GameObject playerKiller;

    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);

        if (PV.IsMine && scene.buildIndex == 1)
            Camera.main.GetComponent<FollowCamera>().target = gameObject.transform;
    }

    void Start()
    {
        PV.Owner.TagObject = gameObject;
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    void Update()
    {
        if (GM == null)
            GM = GameObject.Find("GameManager(Clone)");
    }
    

    public void TakeDamage(float amount)
    {
        //if (!PV.IsMine)
        //    return;

        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        m_CurrentHealth -= amount;

        SetHealthUI();

        if (m_CurrentHealth <= 0f && !m_Dead)
            OnDeath();
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.

        m_Slider.value = m_CurrentHealth;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;

        if (GM.GetComponent<GameManager>().ReturnPlayersLeft() >= 2) //TODO: Here goes a 2, set to 1 for tests 
        {
            //popup.GetComponentInChildren<Text>().enabled = true;
            //popup.GetComponentInChildren<Text>().text = "You were killed by " + name;

            //show spectate button
            //GameObject spectate = GameObject.Find("Spectate");
            //spectate.GetComponent<Image>().enabled = true;
            //spectate.GetComponent<Button>().enabled = true;
            //spectate.GetComponentInChildren<Text>().enabled = true;

            //Spectate
            //GameObject target = GameObject.Find(Spectate() + "(Clone)");
            //Camera.main.GetComponent<FollowCamera>().target = target.transform;

            //GameObject killerName = GameObject.Find("KillerName");
            //killerName.GetComponent<Text>().enabled = true;
            //killerName.GetComponent<Text>().text = target.GetComponent<PhotonView>().Owner.NickName;

            //show exit button
            GameObject exit = GameObject.Find("Exit");
            exit.GetComponent<Image>().enabled = true;
            exit.GetComponent<Button>().enabled = true;
            exit.GetComponentInChildren<Text>().enabled = true;
        }

        //Notify game manager you died
        GM.GetComponent<GameManager>().OnPlayerDeath(PhotonNetwork.LocalPlayer.ActorNumber);

        //Camera follow killer
        //playerKiller = PhotonNetwork.CurrentRoom.GetPlayer(killer).TagObject as GameObject; //get killer
        //Camera.main.GetComponent<FollowCamera>().target = playerKiller.transform; //follow killer
        //Camera.main.GetComponent<FollowCamera>().distance += 10; //set new camera pos

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(m_CurrentHealth);

        else
            m_CurrentHealth = (float)stream.ReceiveNext();
    }
}