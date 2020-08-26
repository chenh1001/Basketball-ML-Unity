using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class playingBallAgent : Agent
{
    public float speed = 600;
    public int range = 10;
    public int counter = 0;
    public bool testing = false;
    Rigidbody ballRgd;
    public bool allReadyHit = false;
    public Transform basket;
    public Transform player;
    /*public override void InitializeAgent()
    {
        ballRgd = gameObject.GetComponent<Rigidbody>();
        GameObject environment = gameObject.transform.parent.gameObject;
        basket = environment.transform.GetChild(4);
        player = environment.transform.GetChild(2);
        Debug.Log(basket);
        Debug.Log(player);
        gameObject.transform.localPosition = new Vector3(player.localPosition.x + 0.4f, player.localPosition.y + 0.4f, player.localPosition.z);
    }*/
    void Start()
    {
        ballRgd = gameObject.GetComponent<Rigidbody>();
        GameObject environment = gameObject.transform.parent.gameObject;
        basket = environment.transform.GetChild(4);
        player = environment.transform.GetChild(2);
        Debug.Log(basket);
        Debug.Log(player);
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        ballRgd.useGravity = true;
        gameObject.transform.localPosition = new Vector3(player.localPosition.x + 0.5f, player.localPosition.y + 0.4f, player.localPosition.z);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(basket.localPosition.x - gameObject.transform.localPosition.x);
        sensor.AddObservation(basket.localPosition.z - gameObject.transform.localPosition.z);
        sensor.AddObservation(basket.localPosition.y - gameObject.transform.localPosition.y);
        sensor.AddObservation(gameObject.transform.localPosition);
        sensor.AddObservation(basket.localPosition);
        sensor.AddObservation(ballRgd.velocity);
        sensor.AddObservation(ballRgd.angularVelocity);
        /*Vector3 dir = basket.position-this.transform.position;
        Ray ray = new Ray(transform.position, new Vector3(dir.x,0,dir.z));
        bool flag = false;
        if (Physics.Raycast(ray, out RaycastHit hit, range/2))
        {
            if (hit.collider.tag == "hoop")
            {
                Debug.Log("CLOSE TO BOARD");
                AddVectorObs(1);
                flag = true;
            }
        }
        if (!flag && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit2, range))
        {
            if (hit2.collider.tag == "target")
            {
                Debug.Log("above target");
                AddVectorObs(10);
                flag = true;
            }
        }
        if (flag == false)
        {
            AddVectorObs(0);
        }*/
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //if(counter==2)
        //    Debug.Log("acting");
        /*Vector3 shot = Vector3.zero;
        shot.x = vectorAction[0];
        shot.z = vectorAction[1];
        shot.y = vectorAction[2];
        //if(allReadyHit==false)
        //{
        ballRgd.useGravity = true;
        allReadyHit = true;
        ballRgd.AddForce(shot * speed);
        */
        allReadyHit = true;
        ballRgd.useGravity = true;
        vectorAction[0] = Mathf.Clamp(vectorAction[0], 0f, 1f);
        vectorAction[1] = Mathf.Clamp(vectorAction[1], 0f, 1f);
        vectorAction[2] = Mathf.Clamp(vectorAction[2], -1f, 1f);
        float x = vectorAction[0];
        float y = vectorAction[1] * 5;
        float z = vectorAction[2];
        ballRgd.AddForce(new Vector3(x, y, z) * speed);
        /*for (int i = 0; i < vectorAction.Length-1; i++)
        {
            vectorAction[i] = Mathf.Clamp(vectorAction[i], -1f, 1f);
        }
        float dirX= vectorAction[0];
        float dirY = vectorAction[1];
        float dirZ = vectorAction[2];
        float power = vectorAction[3] * speed;
        ballRgd.AddForce(new Vector3(dirX,dirY,dirZ)*power);*/
        counter++;
    }
    /*public override void AgentReset()
    {
        counter = 0;
        allReadyHit = false;
        GameObject environment = gameObject.transform.parent.gameObject;
        //player.localPosition = new Vector3(Random.Range(-5f, -10f), 9.6f, Random.Range(-35f, -27f));
        //player.localPosition = new Vector3(Random.Range(-6, -8), 9.6f, Random.Range(-34, -29));
        //player.localPosition = new Vector3(-6, 9.6f, -31.4f);
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        //gameObject.GetComponent<SphereCollider>().isTrigger = true;
        gameObject.transform.localPosition = new Vector3(player.localPosition.x + 0.4f, player.localPosition.y + 0.4f, player.localPosition.z);
        ballRgd.useGravity = true;
    }*/
    void FixedUpdate()
    {
        //AddReward(-0.0001f);
        if (counter <= 1)
        {
            RequestDecision();
        }
        //float dis = Mathf.Sqrt(Mathf.Pow(this.transform.localPosition.x - basket.transform.localPosition.x, 2) + (Mathf.Pow(this.transform.localPosition.z - basket.transform.localPosition.z, 2)));
        if (this.transform.localPosition.x >= basket.localPosition.x + 0.5f)
        {
            float dis = Vector3.Distance(this.gameObject.transform.localPosition, basket.localPosition);
            if (dis >= 1f)
            {
                dis = 1f;
            }
            if (basket.localPosition.y > this.gameObject.transform.localPosition.y)
                dis = 1f;
            AddReward(-dis);
            Destroy(gameObject);
            EndEpisode();
            return;
        }
        if (gameObject.transform.localPosition.z > -26 || gameObject.transform.localPosition.z < -37)
        {
            AddReward(-1.0f);
            EndEpisode();
            Destroy(gameObject);
            return;
        }
        if (gameObject.transform.localPosition.y <= 9.5 || gameObject.transform.localPosition.y >= 18)
        {
            AddReward(-1.0f);
            EndEpisode();
            Destroy(gameObject);
            return;
        }
        if (gameObject.transform.localPosition.x > 1 || gameObject.transform.localPosition.x < -11)
        {
            AddReward(-1.0f);
            EndEpisode();
            Destroy(gameObject);
            return;
        }
        if (gameObject.transform.localPosition.x < player.localPosition.x)
        {
            AddReward(-1.0f);
            EndEpisode();
            Destroy(gameObject);
            return;
        }
    }





    /*void OnTriggerExit(Collider col)
    {
        if (col.gameObject.name == "Player")
        {
            ballRgd.useGravity = true;
            GetComponent<SphereCollider>().isTrigger = false;
        }
    }*/
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("target"))
        {
            Debug.Log("BASKET!");
            AddReward(1.0f);
            EndEpisode();
            return;
        }
        /*if (col.gameObject.CompareTag("board"))
        {
            AddReward(-1.0f);
        }
        if (col.gameObject.CompareTag("hoop"))
        {
            AddReward(-0.5f);
        }*/
    }

}
