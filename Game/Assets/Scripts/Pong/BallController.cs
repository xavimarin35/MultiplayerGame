using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class BallController : MonoBehaviour
{
    Rigidbody2D myRb;
    bool setSpeed;

    [SerializeField] float speedUp;

    public float xSpeed, ySpeed;

    public Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if(!setSpeed)
        {
            setSpeed = true;

            xSpeed = Random.Range(10f, 12f) * 2;
            ySpeed = Random.Range(10f, 12f) * 2;
        }

        MoveBall();
    }

    void MoveBall()
    {
        myRb.velocity = new Vector2(xSpeed, ySpeed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Wall")
        {
            ySpeed = ySpeed * -1;
        }

        if (other.transform.tag == "Paddle")
        {
            xSpeed = xSpeed * -1;

            if (ySpeed > 0)
                xSpeed += speedUp;

            else
                ySpeed -= speedUp;

            if (xSpeed > 0)
                xSpeed += speedUp;

            else
                xSpeed -= speedUp;
        }

        if (other.transform.tag == "EndTwo")
        {
            GameController.instance.scoreOne++;
            GameController.instance.textOne.text = GameController.instance.scoreOne.ToString();

            setSpeed = false;
            myRb.velocity = Vector2.zero;
            this.transform.position = initialPos;
        }
        else if (other.transform.tag == "EndOne")
        {
            GameController.instance.scoreTwo++;
            GameController.instance.textTwo.text = GameController.instance.scoreTwo.ToString();

            setSpeed = false;
            myRb.velocity = Vector2.zero;
            this.transform.position = initialPos;
        }
    }
}
