using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FollowCamera : MonoBehaviourPun
{
    public Transform target;
    public float distance = 17.0f;
    public float height = 8.0f;
    public Vector3 offset;

    public float heightDmp = 2.0f;
    public float rotateDmp = 3.0f;

    GameObject GM;
    GameObject player;

    void LateUpdate()
    {
        if (GM == null)
            GM = GameObject.Find("GameManager(Clone)");

        if (!target)
        {
            player = GM.GetComponent<GameManager>().ReturnPlayerAlive();

            // Dead
            if(!player.activeInHierarchy)
            {
                // Find another tank to spectate
                player = GameObject.Find(GM.GetComponent<GameManager>().GetWinner() + "(Clone)");
            }

            if (player)
                target = player.transform;
        }

        float wantedA = target.eulerAngles.y;
        float wantedH = target.position.y + height;

        float currentA = transform.eulerAngles.y;
        float currentH = transform.position.y;

        currentA = Mathf.LerpAngle(currentA, wantedA, rotateDmp * Time.deltaTime);
        currentH = Mathf.Lerp(currentH, wantedH, heightDmp * Time.deltaTime);

        var currentR = Quaternion.Euler(0, currentA, 0);

        transform.position = target.position;
        transform.position -= currentR * Vector3.forward * (distance + offset.z);

        transform.position = new Vector3(transform.position.x, currentH, transform.position.z);

        transform.LookAt(target);

        //offset = transform.position - target.position;

        //Vector3 newPos = target.position + offset;

        //transform.position = Vector3.Slerp(transform.position, newPos, smooth);

        //if (LookAtPlayer)
        //    transform.LookAt(target);
    }
}
