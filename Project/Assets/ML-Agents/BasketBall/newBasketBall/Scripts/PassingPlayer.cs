using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class PassingPlayer : Agent
{
    public float agentSpeed = 5;
    public float speed = 600;
    Transform ball;
    Rigidbody ballRgd;
    public int counter = 2;
    public Transform teammate;
    public bool hasBall = true;
    //public Transform enemy1;
    //public Transform enemy2;
    //public Transform enemy3;

    public override void Initialize()//reset vals
    {
        GameObject environment = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        ballRgd = environment.transform.GetChild(1).GetComponent<Rigidbody>();
        ball = environment.transform.GetChild(1).transform;
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        teammate = gameObject.transform.parent.GetChild(1);
    }

    public override void CollectObservations(VectorSensor sensor)//get vector of teamate
    {
        float x = ball.localPosition.x;
        float z = ball.localPosition.z;
        sensor.AddObservation(teammate.localPosition.x - x);//relative x 1
        sensor.AddObservation(teammate.localPosition.z - z);//relative z 1
        sensor.AddObservation(teammate.localPosition.y - ball.localPosition.y);//relative y 1
    }

    public override void OnEpisodeBegin()//reset env when a new episode has begun
    {
        hasBall = true;
        counter = 2;
        teammate.localPosition = new Vector3(Random.Range(0f, 19f), Random.Range(1f, 4.7f), Random.Range(-9f, 9f));

        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;

        GetComponent<Rigidbody>().velocity = Vector3.zero;

        teammate.GetComponent<Rigidbody>().useGravity = false;
        teammate.GetComponent<Rigidbody>().velocity = Vector3.zero;
       

        ball.transform.localPosition = new Vector3(transform.localPosition.x + 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
        ballRgd.useGravity = false;
    }

    public override void OnActionReceived(float[] vectorAction) //pass the ball by neural net output
    {
        ballRgd.useGravity = true;
        float x = vectorAction[0];
        float y = vectorAction[1] * 5;
        float z = vectorAction[2];
        ballRgd.AddForce(new Vector3(x, y, z) * speed);
        counter++;
        hasBall = false;
    }

    public void jump()
    {
        if (transform.position.y <= 1)
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 440, 0));
    }

    void FixedUpdate()
    {
        if (hasBall)//update ball pos if has ball
        {
            ball.transform.localPosition = new Vector3(transform.localPosition.x + 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
            ballRgd.angularVelocity = Vector3.zero;
            ballRgd.velocity = Vector3.zero;
        }
        if (counter <= 0)//call desicion
        {
            RequestDecision();
        }
        if (ball.localPosition.z > 10 || ball.localPosition.z < -10)//call if out of bounds
        {
            EndEpisode();
            return;
        }
        if (ball.localPosition.x > 23 || ball.localPosition.x < -23)
        {
            EndEpisode();
            return;
        }
        if (ball.localPosition.y >= 16 || ball.localPosition.y <= 0.6)
        {
            EndEpisode();
            return;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * agentSpeed);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            jump();
        }
        if (Input.GetKey(KeyCode.A) && hasBall)
        {
            counter = 0;
        }
    }

    public void passFailed()
    {
        EndEpisode();
        return;
    }

    public void passMade()
    {
        Debug.Log("PASS COMPLETE");
        EndEpisode();
        return;
    }

}
