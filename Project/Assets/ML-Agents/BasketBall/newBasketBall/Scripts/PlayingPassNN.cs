using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class PlayingPassNN : Agent
{
    public float speed = 600;
    public Transform ball;
    Rigidbody ballRgd;
    public int counter = 0;
    public Transform teammate;
    public int right;
    public ShootingPlayerScript manager;
    public GameObject parent;
    BehaviorParameters behaviorParameters;

    public override void Initialize()//reset vals
    {
        behaviorParameters = GetComponent<BehaviorParameters>();
        right = 1;
        parent.GetComponent<Rigidbody>().useGravity = true;
        GameObject environment = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        //ballRgd = environment.transform.GetChild(1).GetComponent<Rigidbody>();
        ballRgd = ball.GetComponent<Rigidbody>();//environment.transform.GetChild(1).transform;
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        counter = 0;
        //ResetLocations();
        manager = transform.GetComponentInParent<ShootingPlayerScript>();
        manager.hasBall = false;
    }

    public void ResetLocations()//reset locations of player and teamate
    {
        //transform.localPosition = new Vector3(0, Random.Range(1f, 4.7f), 0);
        teammate.transform.localPosition = getRandLoc();
        //transform.LookAt(new Vector3(teammate.position.x, transform.position.y, teammate.position.z));
        ball.transform.localPosition = transform.localPosition + transform.forward * 1.1f + Vector3.up * 0.75f;//new Vector3(transform.localPosition.x + right * 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
    }

    public Vector3 getRandLoc()//get a random location for the teamate not too close to player
    {
        float x = Random.Range(0f, 18f);
        float y = Random.Range(1f, 4.7f);
        float z = Random.Range(-9f, 9f);
        if (x >= -2 && x <= 2 && (z < 3 && z > -3))
            return getRandLoc();
        return new Vector3(x, y, z);
    }

    public override void CollectObservations(VectorSensor sensor)//get information from enviroment - vector from teamate
    {
        float x = right * ball.localPosition.x;
        float z = right * ball.localPosition.z;
        sensor.AddObservation(right * teammate.localPosition.x - x);//relative x 1
        sensor.AddObservation(right * teammate.localPosition.z - z);//relative z 1
        sensor.AddObservation(teammate.localPosition.y - ball.localPosition.y);//relative y 1
    }

    public override void OnActionReceived(float[] vectorAction)//shoot the ball using the neural nets output
    {
        manager.hasBall = false;
        if (manager.gc != null)
            manager.gc.PlayerWithBall = null;
        ballRgd.useGravity = true;
        float x = vectorAction[0];
        float y = vectorAction[1] * 5;
        float z = vectorAction[2];
        ballRgd.AddForce(new Vector3(right * x, y, right * z) * speed);
        EndEpisode();
    }

    void Update()
    {
        if (ball.transform.localPosition.z > 10 || ball.transform.localPosition.z < -10)//check if ball is out of court 
        {
            //AddReward(-1.0f);
            if (manager.gc == null)
                manager.hasBall = true;
        }
        if (ball.transform.localPosition.y >= 16) //|| ball.transform.localPosition.y <= 0.9)
        {
            //AddReward(-1.0f);
            if (manager.gc == null)
                manager.hasBall = true;
        }
        if (ball.transform.localPosition.x > 23 || ball.transform.localPosition.x < -23)
        {
            //AddReward(-1.0f);
            if (manager.gc == null)
                manager.hasBall = true;
        }

    }

    public void pass(Transform ally)
    {
        //Debug.Log(manager.gameObject.name + "PASSES TO "+ ally.gameObject.name);
        teammate = ally;
        RequestDecision();
    }

    public void passMade()
    {
        //AddReward(1.0f);
        if (manager.gc == null)
            manager.hasBall = true;
        return;
    }
}
