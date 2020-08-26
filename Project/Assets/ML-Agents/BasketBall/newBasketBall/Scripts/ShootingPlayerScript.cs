 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPlayerScript : MonoBehaviour
{
    public float speed = 600;
    public float agentSpeed = 5;
    public GameObject ball;
    Rigidbody ballRgd;
    public bool hasBall = false;
    public int right = 1;
    public PlayingPassNN passing;
    public PlayingShotNN shooting;
    public float timer = 0f;
    public gameController gc;

    void Start()
    {
        GameObject environment = gameObject.transform.parent.gameObject;
        ballRgd = ball.GetComponent<Rigidbody>();
        passing = transform.Find("Passing").GetComponent<PlayingPassNN>();
        shooting = transform.Find("Shooting").GetComponent<PlayingShotNN>();
    }

    public void jump()
    {
        if (transform.position.y <= 1)
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 440, 0));
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (hasBall)//update ball values
        {
            ball.transform.localPosition = new Vector3(transform.localPosition.x + right * 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
            ballRgd.angularVelocity = Vector3.zero;
            ballRgd.velocity = Vector3.zero;
        }
        //move funcs

        if (Input.GetKey(KeyCode.RightArrow) && right == 1)
        {
            transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.LeftArrow) && right == 1)
        {
            transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.UpArrow) && right == 1)
        {
            transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow) && right == 1)
        {
            transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * agentSpeed);
        }

        if (Input.GetKey(KeyCode.K) && right == -1)
        {
            transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.H) && right == -1)
        {
            transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.U) && right == -1)
        {
            transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.J) && right == -1)
        {
            transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * agentSpeed);
        }


        if (Input.GetKey(KeyCode.Space))
        {
            jump();
        }
        if (Input.GetKey(KeyCode.S) && hasBall)
        {
            shooting.shoot();
            timer = 1f;
        }
        if (Input.GetKey(KeyCode.A) && hasBall)
        {
            passing.pass(passing.teammate);
            timer = 1f;
        }
    }

    public void madeBasket()//called when a basket is made
    {
        shooting.basketMade();
        return;
    }

    public void madePass()//called when a pass is made
    {
        passing.passMade();
        return;
    }

    public void ResetBall()//resets the ball position
    {
        ball.transform.localPosition = new Vector3(0, 5, 0);
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision col)//called when entering a collision
    {
        if (col.gameObject.CompareTag("shot"))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            if (timer <= 0)
            {
                if (gc != null)
                {
                    if (gc.PlayerWithBall)
                    {
                        gc.PlayerWithBall.GetComponent<ShootingPlayerScript>().timer = 1f;
                        gc.PlayerWithBall.GetComponent<ShootingPlayerScript>().hasBall = false;
                    }
                    gc.PlayerWithBall = this.gameObject;
                }
                timer = 1f;
                hasBall = true;
            }
        }
    }
}
