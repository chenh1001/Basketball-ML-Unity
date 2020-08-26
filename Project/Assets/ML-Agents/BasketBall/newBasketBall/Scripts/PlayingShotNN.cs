using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class PlayingShotNN : Agent
{
    public float speed = 600;
    public GameObject ball;
    Rigidbody ballRgd;
    public int counter = 0;
    public int right = 1;
    public Transform basket;
    public double timer = 10f;
    public ShootingPlayerScript manager;
    BehaviorParameters behaviorParameters;

    public override void Initialize()//reset vars
    {
        behaviorParameters = GetComponent<BehaviorParameters>();
        GameObject environment = gameObject.transform.parent.gameObject;
        ballRgd = ball.GetComponent<Rigidbody>();
        //basket = environment.transform.GetChild(right == 1 ? 3 : 4).GetChild(4);
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        manager = transform.GetComponentInParent<ShootingPlayerScript>();
        counter = 0;
        //un-comment if using inference only shooting
        //transform.localPosition = new Vector3(Random.Range(3f, 18f), 1f, Random.Range(-9f, 9f));
        //ball.transform.localPosition = new Vector3(transform.localPosition.x + right * 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
    }

    public override void CollectObservations(VectorSensor sensor)//get information from enviroment - vector from basket
    {
        float x = right * ball.transform.localPosition.x;
        float z = right * ball.transform.localPosition.z;
        sensor.AddObservation(right * basket.position.x - x);//relative x
        sensor.AddObservation(basket.position.z - z);//relative z
        sensor.AddObservation(basket.position.y - ball.transform.localPosition.y);//relative v
        Vector3 newVel = new Vector3(right * ballRgd.velocity.x, ballRgd.velocity.y, right * ballRgd.velocity.z);
        sensor.AddObservation(newVel);//velocity
        Vector3 newVel2 = new Vector3(right * ballRgd.angularVelocity.x, ballRgd.angularVelocity.y, right * ballRgd.angularVelocity.z);
        sensor.AddObservation(newVel2);//angularVelocity
    }


    public override void OnActionReceived(float[] vectorAction)//shoot the ball using the neural nets output
    {
        manager.hasBall = false;
        if (manager.gc != null)
            manager.gc.PlayerWithBall = null;
        float x = vectorAction[0];
        float y = vectorAction[1] * 5;
        float z = vectorAction[2];
        ballRgd.AddForce(new Vector3(right * x, y, right * z) * speed);
        EndEpisode();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (ball.transform.localPosition.z > 10 || ball.transform.localPosition.z < -10)//check if ball is out of court 
        {
            //AddReward(-1.0f);
            if (manager.gc == null)
                manager.hasBall = true;

        }
        if (ball.transform.localPosition.y >= 16 || ball.transform.localPosition.y <= -1) //|| ball.transform.localPosition.y <= 0.9)
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

    public void shoot()//shoot the ball
    {
        RequestDecision();
        // un comment if using BasketBallShooterPlayer
        /*if (counter > 1 && GetComponent<BasketBallShooterPlayer>() != null && GetComponent<BasketBallShooterPlayer>().hasBall)
        {
            GetComponent<BasketBallShooterPlayer>().hasBall = false;
            counter = 0;
            GetComponent<BasketBallShooterPlayer>().timer = 1;
            if (gc != null)
                gc.PlayerWithBall = null;
        }
        else if (counter > 1 && GetComponent<ShootingPlayerScript>() != null && GetComponent<ShootingPlayerScript>().hasBall)
        {
            GetComponent<ShootingPlayerScript>().hasBall = false;
            counter = 0;
        }*/
    }

    public void basketMade()//called when basket is made, return ball to player
    {
        if (manager.gc == null)
            manager.hasBall = true;
        return;
    }
}
