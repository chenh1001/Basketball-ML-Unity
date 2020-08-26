using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class Full_train_nn : Agent
{

    public enum Team
    {
        Blue = 0,
        Red = 1
    }

    [HideInInspector]
    public Team team;
    //[HideInInspector]
    public float timePenalty;
    [HideInInspector]
    public float highJumpHeight = 700f;
    [HideInInspector]
    public float lowJumpHeight = 550f;

    public float jumpHeight;
    public float speed = 600;
    public float agentSpeed = 5f;
    public GameObject ball;
    Rigidbody ballRgd;
    public float timer = 0f;
    public bool hasBall = false;
    public int right = 1;
    public PlayingPassNN passing;
    public PlayingShotNN shooting;
    public gameController gc;
    public Transform basket;
    float existential;
    float ballTouch;
    EnvironmentParameters resetParams;

    BehaviorParameters behaviorParameters;
    Vector3 startingLoc;
    Rigidbody agentRb;
    float forwardSpeed = 0.8f;
    public Transform teamate1;
    //public Transform teamate2;
    public int actionMaker = 0;
    bool flag = false;

    public override void Initialize()//reset vals
    {
        existential = 1.0f / MaxStep;
        timePenalty = 0;

        behaviorParameters = GetComponent<BehaviorParameters>();
        passing = transform.Find("Passing").GetComponent<PlayingPassNN>();
        shooting = transform.Find("Shooting").GetComponent<PlayingShotNN>();
        basket = shooting.basket;
        ballRgd = ball.GetComponent<Rigidbody>();
        agentRb = GetComponent<Rigidbody>();

        if (behaviorParameters.TeamId == (int)Team.Blue)//assign a team
        {
            team = Team.Blue;
            startingLoc = new Vector3(transform.localPosition.x + 4f, 1f, transform.localPosition.z);
        }
        else
        {
            team = Team.Red;
            startingLoc = new Vector3(transform.localPosition.x - 4f, 1f, transform.localPosition.z);
        }
        transform.localPosition = startingLoc;
        agentRb.velocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        jumpHeight = lowJumpHeight;
        agentRb.maxAngularVelocity = 500;

        var playerConfig = new gameController.PlayerConfig
        {
            agentRb = agentRb,
            startingPos = transform.localPosition,
            agentScript = this,
        };
        gc.playerConfigs.Add(playerConfig); //add self to gc players list
        resetParams = Academy.Instance.EnvironmentParameters;

    }

    public override void CollectObservations(VectorSensor sensor)//get information from enviroment
    {
        sensor.AddObservation(ball.transform.localPosition.x - transform.localPosition.x);//relative x ball 1 
        sensor.AddObservation(ball.transform.localPosition.z - transform.localPosition.z);//relative z ball 1
        sensor.AddObservation(ball.transform.localPosition.y - transform.localPosition.y);//relative y ball 1

        sensor.AddObservation(teamate1.localPosition.x - transform.localPosition.x);//pos x 1
        sensor.AddObservation(teamate1.localPosition.z - transform.localPosition.z);//pos z 1
        sensor.AddObservation(teamate1.localPosition.y - transform.localPosition.y);//pos y 1
        // UN COMENT IF WANT TO LEARN BY VALS NOT SENSORS

        /*if (hasBall)
        {
            ball.transform.localPosition = transform.localPosition + transform.forward * 1.1f + Vector3.up * 0.75f;
            ballRgd.angularVelocity = Vector3.zero;
            ballRgd.velocity = Vector3.zero;
        }

        sensor.AddObservation(basket.localPosition.x - transform.localPosition.x);//relative x basket 1
        sensor.AddObservation(basket.localPosition.z - transform.localPosition.z);//relative z basket 1 
        sensor.AddObservation(basket.localPosition.y - transform.localPosition.y);//relative y basket 1 

        sensor.AddObservation(ball.transform.localPosition.x - transform.localPosition.x);//relative x ball 1 
        sensor.AddObservation(ball.transform.localPosition.z - transform.localPosition.z);//relative z ball 1
        sensor.AddObservation(ball.transform.localPosition.y - transform.localPosition.y);//relative y ball 1

        Vector3 newVel = new Vector3(ballRgd.velocity.x, ballRgd.velocity.y, ballRgd.velocity.z);
        sensor.AddObservation(newVel);//ball velocity 3
        Vector3 newVel2 = new Vector3(ballRgd.angularVelocity.x, ballRgd.angularVelocity.y, ballRgd.angularVelocity.z);
        sensor.AddObservation(newVel2);//ball angularVelocity 3

        sensor.AddOneHotObservation((int)team, 2);//team num 1
        sensor.AddObservation(transform.rotation);//rotation 4
        sensor.AddObservation(agentRb.velocity); //agent velocity 3
        sensor.AddObservation(agentRb.angularVelocity);// agent angularVelocity 3
        sensor.AddObservation(hasBall ? 1 : 0);//agent has ball 1

        Transform[] teamates = { teamate1, teamate2 };
        foreach ( Transform teamate in teamates)
        {
            Full_train_nn script = teamate.GetComponent<Full_train_nn>();
            sensor.AddOneHotObservation((int)script.team, 2);//team num 1

            sensor.AddObservation(teamate.localPosition.x - transform.localPosition.x);//pos x 1
            sensor.AddObservation(teamate.localPosition.z - transform.localPosition.z);//pos z 1
            sensor.AddObservation(teamate.localPosition.y - transform.localPosition.y);//pos y 1

            sensor.AddObservation(script.transform.rotation);//rotation 4

            sensor.AddObservation(script.agentRb.velocity);//vel 3
            sensor.AddObservation(script.agentRb.angularVelocity);//vel 3

            sensor.AddObservation(script.hasBall ? 1 : 0);//has ball 1
        }
        foreach (var ps in gc.playerConfigs) // *3
        {
            if (ps.agentScript.team != team)
            {
                sensor.AddOneHotObservation((int)ps.agentScript.team, 2);//team num 1

                sensor.AddObservation(ps.agentScript.transform.localPosition.x - transform.localPosition.x);//pos x 1
                sensor.AddObservation(ps.agentScript.transform.localPosition.z - transform.localPosition.z);//pos z 1
                sensor.AddObservation(ps.agentScript.transform.localPosition.y - transform.localPosition.y);//pos y 1

                sensor.AddObservation(ps.agentScript.transform.rotation);//rotation 4

                sensor.AddObservation(ps.agentScript.agentRb.velocity);//vel 3
                sensor.AddObservation(ps.agentScript.agentRb.angularVelocity);//vel 3

                sensor.AddObservation(ps.agentScript.hasBall ? 1 : 0);//has ball 1
            }
        }*/

        /*sensor.AddObservation(hasBall);
        //sensor.AddObservation(jumpHeight);
        sensor.AddObservation((basket.localPosition.x - transform.localPosition.x)/60.0f);//relative x basket 1
        sensor.AddObservation((basket.localPosition.z - transform.localPosition.z)/20.0f);//relative z basket 1 
        sensor.AddObservation(basket.localPosition.y - transform.localPosition.y);//relative y basket 1 
         
        sensor.AddObservation((ball.transform.localPosition.x - transform.localPosition.x)/60.0f);//relative x basket 1
        sensor.AddObservation((ball.transform.localPosition.z - transform.localPosition.z)/20.0f);//relative z basket 1 
        sensor.AddObservation(ball.transform.localPosition.y - transform.localPosition.y);//relative y basket 1 */
    }

    public override void OnActionReceived(float[] vectorAction)//output of neural network
    {
        /*
         * 0 - size = 3    0 - nothing 1 - move forward 2 - move backward
         * 1 - size = 3    0 - nothing 1 - move left 2 - move right
         * 2 - size = 3    0 - nothing 1 - turn left 2 - turn right
         * 3 - size = 2    0 - nothing 1 - jump
         * 4 - size = 4    0 - nothing 1 - shoot 2 - pass to one 3 - pass to two 
         */
        timePenalty -= existential;
        //AddReward(-1 * existential);
        MoveAgent(vectorAction);
    }

    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)//make masks on actions
    {
        if (!hasBall)//mask shooting and passing if doesnt have ball
        {
            actionMasker.SetMask(3, new int[2] { 1, 2 });
        }
        //if (transform.position.y > 1)
        //    actionMasker.SetMask(3, new int[1] { 1 });
    }

    public void MoveAgent(float[] act)//move agent by neural net
    {
        var actionArr = (int)act[3];

        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var forwardAxis = (int)act[0];
        var rightAxis = (int)act[1];
        var rotateAxis = (int)act[2];
        //UN COMMENT TO MOVE BY FORCE
        //var jumpArr = (int)act[3];
        
        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * forwardSpeed;
                break;
            case 2:
                dirToGo = transform.forward * -forwardSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right * forwardSpeed;
                break;
            case 2:
                dirToGo = transform.right * -forwardSpeed;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }

        /*switch (jumpArr)
        {
            case 1:
                jump();
                break;
        }*/

        transform.Rotate(rotateDir, Time.deltaTime * 500f);
        agentRb.AddForce(dirToGo * agentSpeed,ForceMode.VelocityChange);

        //move by adding vectors
        /*switch (forwardAxis)
        {
            case 1:
                transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * agentSpeed);
                break;
            case 2:
                transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * agentSpeed);
                break;
        }
        switch (rightAxis)
        {
            case 1:
                transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * agentSpeed);
                break;
            case 2:
                transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * agentSpeed);
                break;
        }
        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 500f);
        */
        switch (actionArr)
        {
            case 1:
                actionMaker = 1;//Shoot
                break;
            case 2:
                actionMaker = 2;//Pass
                break;
            /*case 3:

                actionMaker = 3;
                //break;
                */
        }
    }

    //brain used when human plays
    public override void Heuristic(float[] actionsOut)
    {
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, 0.4f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, 0.4f);
        }
        //rotate
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -1 * 7, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 1 * 7, 0);
        }
        actionsOut[3] = 0f;
        if (Input.GetKey(KeyCode.Space))
        {
            actionsOut[3] = 1f;
        }
        if (Input.GetKey(KeyCode.P))
        {
            actionsOut[3] = 2f;
        }
    }

    //reset params when a new episode begins
    public override void OnEpisodeBegin()
    {
        timePenalty = 0;
        hasBall = false;
        actionMaker = 0;
        ballTouch = resetParams.GetWithDefault("ball_touch", 0);

        transform.localPosition = startingLoc;
        gc.outOfBounds();

        if (team == Team.Red)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }

        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void Update()
    {
        if(ballTouch == 0 && flag == false)
        {
            Debug.Log("Ball touch is 0 ");
            flag = true;
        }
        timer -= Time.deltaTime;
        if (hasBall)//update ball vals if has ball
        {
            ball.transform.localPosition = transform.localPosition + Vector3.up * 2.5f;//transform.forward * 1.7f + Vector3.up * 1.3f;
            ballRgd.angularVelocity = Vector3.zero;
            ballRgd.velocity = Vector3.zero;
            if (actionMaker == 1)//if wants to shoot shoot
            {
                hasBall = false;
                actionMaker = 0;
                shooting.shoot();
                actionMaker = 0;
                timer = 1f;
            }

            if (actionMaker == 2)//if want to pass pass
            {
                hasBall = false;
                actionMaker = 0;
                passing.pass(teamate1);
                actionMaker = 0;
                timer = 1f;
            }

            /*if (actionMaker == 3)//jump
            {
                hasBall = false;
                passing.pass(teamate2);
                actionMaker = 0;
                timer = 1f;
            }*/
        }
        else
            actionMaker = 0;

        //check if out of bounds if true reset and punish
        if (transform.localPosition.z > 12 || transform.localPosition.z < -12)
        {
            AddReward(-0.05f);
            transform.localPosition = startingLoc;
        }
        if (transform.localPosition.y >= 25 || transform.localPosition.y <= -2)
        {
            AddReward(-0.05f);
            transform.localPosition = startingLoc;
        }
        if (transform.localPosition.x > 33 || transform.localPosition.x < -33)
        {
            AddReward(-0.05f);
            transform.localPosition = startingLoc;
        }
    }

    public void jump()
    {
        if (transform.position.y <= 1)
            GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpHeight * 1.6f, 0),ForceMode.Force);
    }

    void OnCollisionEnter(Collision col)//attach ball on collision with player with ball
    {
        if (col.gameObject.transform.Find("Shooting") && col.gameObject.GetComponent<Full_train_nn>().hasBall && col.gameObject.GetComponent<Full_train_nn>().team != team || col.gameObject.tag.Equals("shot"))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            if (timer <= 0)
            {
                AddReward(.2f * ballTouch);
                gc.changePlayerWithBall(this);
                timer = 1f;
                hasBall = true;
            }
        }
    }

}
