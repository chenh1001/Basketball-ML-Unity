using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basketBallPlayer : MonoBehaviour
{
    public GameObject basketBall;
    Rigidbody rgd;
    public int spawnSpeed = 50;
    int counter = 100;
    public float speed = 50f;
    // Start is called before the first frame update
    void Start()
    {
        rgd = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
   
    void FixedUpdate()
    {
        if (counter >= spawnSpeed)
        {
            GameObject environment = gameObject.transform.parent.gameObject;
            GameObject ball = Instantiate(basketBall);
            ball.transform.parent = environment.transform;
            //ball.GetComponent<playingBallAgent>().InitializeAgent();
            //ball.transform.localPosition = new Vector3(transform.localPosition.x + 0.4f, transform.localPosition.y + 0.4f, transform.localPosition.z);
            Rigidbody rgd = ball.GetComponent<Rigidbody>();
            rgd.velocity = Vector3.zero;
            rgd.angularVelocity = Vector3.zero;
            counter = 0;
        }
        counter++;

        if (Input.GetKey(KeyCode.RightArrow) && transform.localPosition.x < -4.3f)
        {
            transform.Translate(new Vector3(1,0,0) * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * speed);
        }
    }
}
