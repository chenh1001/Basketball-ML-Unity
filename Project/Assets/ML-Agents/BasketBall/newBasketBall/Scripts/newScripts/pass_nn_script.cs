using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class pass_nn_script : Agent
{
    public float speed = 600;
    public Transform ball;
    Rigidbody ballRgd;
    public int counter = 0;
    public Transform teammate;
    public gameController gc;
    public int right;
    bool hasBall;

    public override void Initialize()//reset vars
    {
        //un comment if using BasketBallShooterPlayer
        //right = GetComponent<BasketBallShooterPlayer>().right;
        right = 1;
        GetComponent<Rigidbody>().useGravity = false;
        GameObject environment = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        ballRgd = environment.transform.GetChild(1).GetComponent<Rigidbody>();
        ball = environment.transform.GetChild(1).transform;
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        hasBall = true;
        counter = 0;
        ResetLocations();
    }

    public void ResetLocations()//reset locations of player and teamate
    {
        transform.localPosition = new Vector3(0, Random.Range(1f, 4.7f), 0);
        teammate.transform.localPosition = getRandLoc();
        transform.LookAt(new Vector3(teammate.position.x, transform.position.y, teammate.position.z));
        ball.transform.localPosition = transform.localPosition + transform.forward * 1.1f + Vector3.up * 0.75f;//new Vector3(transform.localPosition.x + right * 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
    }

    public Vector3 getRandLoc()//get a random location for the teamate not too close to player
    {
        float x = Random.Range(-18f, 18f);
        float y = Random.Range(1f, 4.7f);
        float z = Random.Range(-18f, 18f);
        if (x >= -2 && x <= 2 && (z < 3 && z > -3))
            return getRandLoc();
        return new Vector3(x, y, z);
    }

    public override void OnEpisodeBegin()//reset enviroment when episode begins
    {
        //un comment if using BasketBallShooterPlayer
        //GetComponent<ShootingPlayerScript>().hasBall = false;
        hasBall = true;
        counter = 0;
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        teammate.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        teammate.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ResetLocations();
    }

    public override void CollectObservations(VectorSensor sensor)//get vector from teamate to ball
    {
        float x = right * ball.localPosition.x;
        float z = right * ball.localPosition.z;
        sensor.AddObservation(right * teammate.localPosition.x - x);//relative x 1
        sensor.AddObservation(right * teammate.localPosition.z - z);//relative z 1
        sensor.AddObservation(teammate.localPosition.y - ball.localPosition.y);//relative y 1
    }

    public override void OnActionReceived(float[] vectorAction)//pass the ball by the neural nets output
    {
        ballRgd.useGravity = true;
        float x = vectorAction[0];
        float y = vectorAction[1] * 5;
        float z = vectorAction[2];
        ballRgd.AddForce(new Vector3(right * x, y, right * z) * speed);
        counter++;
    }

    void FixedUpdate()
    {
        //AddReward(-0.0001f);
        if (counter <= 0)
        {
            RequestDecision();
        }
        if (ball.localPosition.z > 20 || ball.localPosition.z < -20)//reset ball if out of bounds
        {
            AddReward(-1.0f);
            EndEpisode();
            return;
        }
        if (ball.localPosition.x > 23 || ball.localPosition.x < -23)
        {
            AddReward(-1.0f);
            EndEpisode();
            return;
        }
        if (ball.localPosition.y >= 16 || ball.localPosition.y <= 0.6)
        {
            AddReward(-1.0f);
            EndEpisode();
            return;
        }
    }

    public void Pass()//pass the ball to the teamate
    {
        counter = 0;
        /*if (counter > 1 && GetComponent<BasketBallShooterPlayer>().hasBall)
        {
            counter = 0;
            GetComponent<BasketBallShooterPlayer>().hasBall = false;
            GetComponent<BasketBallShooterPlayer>().timer = 1;
            gc.PlayerWithBall = null;
        }*/
    }


    public void passMade()//called when a pass is made
    {
        AddReward(1.0f);
        EndEpisode();
        return;
    }
}
