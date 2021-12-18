using UnityEngine;


public class PaddleController : MonoBehaviour
{
    public string upKey, downKey;
    public float speed;

    private void Update()
    {
        PaddleMovement();
    }

    void PaddleMovement()
    {
        if(Input.GetKey(upKey) && transform.position.y < 40)
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed, Space.World);
        }
        if (Input.GetKey(downKey) && transform.position.y > -40)
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed, Space.World);
        }
    }
}