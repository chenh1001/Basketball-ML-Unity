using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class learning_to_shoot : Agent
{
    public float speed = 600;
    public GameObject ball;
    Rigidbody ballRgd;
    public int counter = 0;
    public int right = 1;
    public Transform basket;
    public gameController gc;

    public override void Initialize()
    {
        GameObject environment = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        ball = environment.transform.GetChild(1).gameObject;
        ballRgd = environment.transform.GetChild(1).GetComponent<Rigidbody>();
        basket = environment.transform.GetChild(right == 1 ? 3 : 4).GetChild(4);
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float x = right * ball.transform.localPosition.x;
        float z = right * ball.transform.localPosition.z;
        sensor.AddObservation(right * basket.localPosition.x - x);//relative x
        sensor.AddObservation(basket.localPosition.z - z);//relative z
        sensor.AddObservation(basket.localPosition.y - ball.transform.localPosition.y);//relative v
        Vector3 newVel = new Vector3(right * ballRgd.velocity.x, ballRgd.velocity.y, right * ballRgd.velocity.z);
        sensor.AddObservation(newVel);//velocity
        Vector3 newVel2 = new Vector3(right * ballRgd.angularVelocity.x, ballRgd.angularVelocity.y, right * ballRgd.angularVelocity.z);
        sensor.AddObservation(newVel2);//angularVelocity
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float x = vectorAction[0];
        float y = vectorAction[1] * 5;
        float z = vectorAction[2];
        ballRgd.AddForce(new Vector3(right * x, y, right * z) * speed);
        counter++;
        if (counter > 1)
            EndEpisode();
    }

    void FixedUpdate()
    {
        if (counter <= 1)
        {
            RequestDecision();
        }
    }

    public void shoot()
    {
        if (counter > 1 && GetComponent<BasketBallShooterPlayer>().hasBall)
        {
            GetComponent<BasketBallShooterPlayer>().hasBall = false;
            counter = 0;
            GetComponent<BasketBallShooterPlayer>().timer = 1;
            gc.PlayerWithBall = null;
        }
    }

    public void basketMade()
    {
        Debug.Log("BALL CALLED MADEBASKET");
        AddReward(1.0f);
        gc.outOfBounds();
        RequestDecision();
        return;
    }
}
