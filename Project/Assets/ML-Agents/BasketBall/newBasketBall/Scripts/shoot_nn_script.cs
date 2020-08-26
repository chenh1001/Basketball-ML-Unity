using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Sensors;

public class shoot_nn_script : Agent
{
    public float speed = 600;
    public GameObject ball;
    Rigidbody ballRgd;
    public int counter = 0;
    public int right = 1;
    public Transform basket;
    public gameController gc;
    bool hasBall = true;
    public double timer = 10f;

    public override void Initialize()// initialize all values
    {
        GetComponent<Rigidbody>().useGravity = false;
        GameObject environment = gameObject.transform.parent.gameObject;
        ballRgd = environment.transform.GetChild(2).GetComponent<Rigidbody>();
        basket = environment.transform.GetChild(right == 1 ? 3 : 4).GetChild(4); //get basket
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;

        hasBall = true;
        counter = 0;
        transform.localPosition = new Vector3(Random.Range(3f, 18f), 1f, Random.Range(-9f, 9f)); //reset location
        ball.transform.localPosition = new Vector3(transform.localPosition.x + right * 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z); //reset ball location
    }

    public override void CollectObservations(VectorSensor sensor) //collect information from enviroment
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

    public Vector3 getRandLoc()//get a random location not under the basket
    {
        float x = Random.Range(5f, 18f);
        float y = Random.Range(1f, 4.7f);
        float z = Random.Range(-9f, 6f);
        if (x >= 16 && (z < 2.5 && z > -2.5))
            return getRandLoc();
        return new Vector3(x, y, z);
    }

    public override void OnEpisodeBegin()//reset all values when a new episode begin
    {
        GetComponent<ShootingPlayerScript>().hasBall = true;
        timer = 10;
        hasBall = false;
        counter = 0;
        transform.localPosition = getRandLoc();
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.localPosition = new Vector3(transform.localPosition.x + right * 1.1f, transform.localPosition.y + 0.75f, transform.localPosition.z);
    }

    public override void OnActionReceived(float[] vectorAction)//shoot ball with given output
    {
        float x = vectorAction[0];
        float y = vectorAction[1] * 5;
        float z = vectorAction[2];
        ballRgd.AddForce(new Vector3(right * x, y, right * z) * speed);
        counter++;
    }
    
    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (counter == 0)
        {
            RequestDecision();
        }
        if(timer <= 0)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        if (ball.transform.localPosition.z > 10 || ball.transform.localPosition.z < -10) //if ball is out of court reset loc 
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        if (ball.transform.localPosition.y >= 16 || ball.transform.localPosition.y <= 0.9)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        if (ball.transform.localPosition.x > 23 || ball.transform.localPosition.x < -23)
        {
            AddReward(-1.0f);
            EndEpisode();
        }

    }

    public void shoot()
    {
        counter = 0;
        //UN COMMENT IF LEARNING
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

    public void basketMade() //called by ball when basket is made 
    {
        AddReward(1.0f);
        if (gc != null)
            gc.outOfBounds();
        EndEpisode();
        return;
    }
}
