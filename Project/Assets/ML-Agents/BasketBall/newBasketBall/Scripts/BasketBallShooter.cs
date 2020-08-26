using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Sensors;


public class BasketBallShooter : Agent
{
    public float speed = 600;
    public GameObject ball;
    Rigidbody ballRgd;
    int counter = 0;
    public bool hasBall = true;
    public Transform basketRight;
    public Transform basketLeft;
    public int right = 1;
    Transform basket;

    public override void Initialize()
    {
        GameObject environment = gameObject.transform.parent.gameObject;
        ballRgd = environment.transform.GetChild(2).GetComponent<Rigidbody>();
        basketRight = environment.transform.GetChild(3).GetChild(4);
        basketLeft = environment.transform.GetChild(4).GetChild(4);

        basket = environment.transform.GetChild(3).GetChild(4);
        ballRgd.useGravity = false;
        ball.transform.localPosition = new Vector3(transform.localPosition.x + 1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
        transform.localPosition = new Vector3(Random.Range(11f, 13f), 1f, Random.Range(-5f, 5f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(basket.localPosition.x - ball.transform.localPosition.x);//relative x
        sensor.AddObservation(basket.localPosition.z - ball.transform.localPosition.z);//relative z
        sensor.AddObservation(basket.localPosition.y - ball.transform.localPosition.y);//relative y
        sensor.AddObservation(ballRgd.velocity);//velocity
        sensor.AddObservation(ballRgd.angularVelocity);//angularVelocity
        //AddVectorObs(counter);
        //AddVectorObs(right);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        ballRgd.useGravity = true;
        //vectorAction[0] = Mathf.Clamp(vectorAction[0], 0f, 1f);
        //vectorAction[1] = Mathf.Clamp(vectorAction[1], 0f, 1f);
        //vectorAction[2] = Mathf.Clamp(vectorAction[2], -1f, 1f);
        
        float x = vectorAction[0];
        float y = vectorAction[1] * 5;
        float z = vectorAction[2];
        ballRgd.AddForce(new Vector3(x, y, z) * speed);
        hasBall = false;
        
        counter++;
    }

    public Vector3 GetNewPos()
    {
        float randomNum = Random.Range(1, 4.5f);
        float x = Random.Range(9f, 18f);
        float z = Random.Range(-7f, 7f);
        if (x >= 15 && z >= -1.5 && z <= 1.5)
        {
            return GetNewPos();
        }
        return new Vector3(x, randomNum, z);
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        counter = 0;
        hasBall = true;
        GameObject environment = gameObject.transform.parent.gameObject;
        float tmp = Random.Range(0.0f, 1.0f);
        //right = tmp > 0.5 ? -1 : 1;
        right = 1;
        Vector3 newPos = GetNewPos();
        newPos.x = newPos.x * right;
        newPos.z = newPos.z * right;
        if (right == 1)
            basket = basketRight;
        else
            basket = basketLeft;
        transform.localPosition = newPos;
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        
        ball.transform.localPosition = new Vector3(transform.localPosition.x + right * 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
        ballRgd.useGravity = false;
    }

    void FixedUpdate()
    {
        if(hasBall)
            ball.transform.localPosition = new Vector3(transform.localPosition.x + right * 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
        
        if (counter <= 1)
        {
            RequestDecision();
        }
        
        if (ball.transform.localPosition.z > 10 || ball.transform.localPosition.z < -10)
        {
            AddReward(-1.0f);
            EndEpisode();
            return;
        }
        if (ball.transform.localPosition.y <= 0.6 || ball.transform.localPosition.y >= 16)
        {
            AddReward(-1.0f);
            EndEpisode();
            return;
        }
        if (ball.transform.localPosition.x > 23 || ball.transform.localPosition.x < -23)
        {
            AddReward(-1.0f);
            EndEpisode();
            return;
        }
    }
    
    public void jump()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0, 440, 0));
    }

    public void madeBasket()
    {
        AddReward(1.0f);
        EndEpisode();
        return;
    }

}
