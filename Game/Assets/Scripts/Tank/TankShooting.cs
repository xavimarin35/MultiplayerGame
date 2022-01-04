using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;

    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;
    private bool holding;

    private PhotonView myPV;

    protected Vector3 aimPoint;
    public GameObject turret;
    public GameObject GM;

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        myPV = GetComponent<PhotonView>();

        m_FireButton = "Fire" + m_PlayerNumber;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    private void Update()
    {
        if (myPV.IsMine)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                aimPoint = new Vector3(hit.point.x, 0.1f, hit.point.z);
            }

            Vector3 direction = aimPoint - turret.transform.position;
            direction.y = 0;

            turret.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Track the current state of the fire button and make decisions based on the current launch force.

            m_AimSlider.value = m_MinLaunchForce;

            if (GM == null)
                GM = GameObject.Find("GameManager(Clone)");

            if (GM.GetComponent<GameManager>().GameStarted())
            {
                // Max Charge, not fired
                if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired && holding)
                {
                    m_CurrentLaunchForce = m_MaxLaunchForce;
                    Fire();
                }
                // Pressed for the first time
                else if (Input.GetMouseButtonDown(0))
                {
                    m_Fired = false;
                    holding = true;
                    m_CurrentLaunchForce = m_MinLaunchForce;

                    m_ShootingAudio.clip = m_ChargingClip;
                    m_ShootingAudio.Play();
                }
                // Holding the fire
                else if (!Input.GetMouseButtonUp(0) && holding && !m_Fired)
                {
                    m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

                    m_AimSlider.value = m_CurrentLaunchForce;
                }
                // Release button
                else if (Input.GetMouseButtonUp(0) && holding)
                {
                    Fire();
                }
            }
            
        }        
    }


    private void Fire()
    {
        // Instantiate and launch the shell.

        holding = false;
        m_Fired = true;

        GameObject bullet = PhotonNetwork.Instantiate("PhotonPrefabs/Shell", m_FireTransform.position, Quaternion.LookRotation(turret.transform.forward));
        Vector3 force = turret.transform.forward * m_CurrentLaunchForce;
        force.y += 3.0f;
        bullet.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        //Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        //shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}