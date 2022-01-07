using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Compass : MonoBehaviour
{
    public RawImage compassImage;
    public GameObject player;
    public int actornum;

    public GameObject iconPrefab;
    List<TankMarker> tankMarkers = new List<TankMarker>();

    float compassUnit;

    public TankMarker blue;
    public TankMarker red;
    public TankMarker yellow;
    public TankMarker green;

    private void Awake()
    {
        actornum = PhotonNetwork.LocalPlayer.ActorNumber;
        compassUnit = compassImage.rectTransform.rect.width / 360f;

        AddTankMarker(blue);
        AddTankMarker(red);
        AddTankMarker(yellow);
        AddTankMarker(green);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        compassImage.uvRect = new Rect(player.transform.localEulerAngles.y / 360f, 0f, 1f, 1f);

        foreach (TankMarker marker in tankMarkers)
        {
            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);
        }
    }

    public void AddTankMarker(TankMarker marker)
    {
        GameObject newMarker = Instantiate(iconPrefab, compassImage.transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        tankMarkers.Add(marker);
    }

    Vector2 GetPosOnCompass (TankMarker marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
