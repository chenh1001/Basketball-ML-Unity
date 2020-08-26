using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{

    [System.Serializable]
    public class PlayerConfig
    {
        public Rigidbody agentRb;
        public Vector3 startingPos;
        public Full_train_nn agentScript;
        public float ballPosReward;
    }

    // Start is called before the first frame update
    public GameObject PlayerWithBall = null;
    public GameObject ball;
    public Full_train_nn lastPlayerWithBall;
    Rigidbody ballRgd;
    public List<PlayerConfig> playerConfigs = new List<PlayerConfig>();

    void Start() //reset vals
    {
        lastPlayerWithBall = null;
        PlayerWithBall = null;
        GameObject environment = gameObject.transform.parent.gameObject;
        ball.transform.localPosition = new Vector3(0, 5, 0);
        ballRgd = ball.GetComponent<Rigidbody>();
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
    }

    public void outOfBounds()//reset the enviroment if ball is out of bounds
    {
        ball.transform.localPosition = new Vector3(0, 5, 0);
        ballRgd.angularVelocity = Vector3.zero;
        ballRgd.velocity = Vector3.zero;
        if (PlayerWithBall != null)
            PlayerWithBall.GetComponent<Full_train_nn>().hasBall = false;
        PlayerWithBall = null;
        lastPlayerWithBall = null;
    }

    public void basketMade()//called when a basket is made
    {
        outOfBounds();
    }

    public void changePlayerWithBall(Full_train_nn withBall) // called by a player when he took over a ball
    {
        if (PlayerWithBall && PlayerWithBall != withBall.gameObject)//minus reward if stolen
        {
            PlayerWithBall.GetComponent<Full_train_nn>().timer = 1f;
            PlayerWithBall.GetComponent<Full_train_nn>().hasBall = false;
            PlayerWithBall.GetComponent<Full_train_nn>().AddReward(-0.1f);
        }
        /*if (lastPlayerWithBall != null && lastPlayerWithBall.team == withBall.team && lastPlayerWithBall != withBall && PlayerWithBall == null)//pass acured
        {
            withBall.AddReward(0.05f);
            lastPlayerWithBall.AddReward(0.05f);
        }
        else if(lastPlayerWithBall != null && lastPlayerWithBall.team != withBall.team)//block
        {
            withBall.AddReward(0.05f);
            lastPlayerWithBall.AddReward(-0.05f);
        }*/
        PlayerWithBall = withBall.gameObject;
        //add reward if was blocked
        if ((lastPlayerWithBall != null && !lastPlayerWithBall.Equals(withBall) && lastPlayerWithBall.team != withBall.team) || lastPlayerWithBall == null)
        {
            /*if (lastPlayerWithBall)
                lastPlayerWithBall.AddReward(-0.1f);
            withBall.AddReward(0.1f);*/
        }
        
        lastPlayerWithBall = withBall;
        /*foreach (var ps in playerConfigs)
        {
            if (ps.agentScript.team == PlayerWithBall.GetComponent<Full_train_nn>().team)
            {
                ps.agentScript.jumpHeight = ps.agentScript.lowJumpHeight;
            }
            else
            {
                ps.agentScript.jumpHeight = ps.agentScript.highJumpHeight;
            }
        }*/
    }

    public void PlayerLeftBall(Full_train_nn player)//update player with ball value when a player leaves the ball
    {
        PlayerWithBall = null;
    }
    //called when a team scored, add reward and negative reward based on team
    public void TeamScored(Full_train_nn.Team scoredTeam)
    {
        if (scoredTeam == Full_train_nn.Team.Blue)
            Debug.Log("BASKET BLUE");
        else
            Debug.Log("BASKET RED");
        foreach (var ps in playerConfigs)
        {
            if (ps.agentScript.team == scoredTeam)
            {
                //if (ps.agentScript.Equals(lastPlayerWithBall))
                //    ps.agentScript.AddReward(1.0f + ps.agentScript.timePenalty);//scored basket
                //else
                ps.agentScript.AddReward(2.0f + ps.agentScript.timePenalty);//ally scored basket
                //ps.agentScript.AddReward(1.0f + ps.agentScript.timePenalty);
            }
            else
            {
                ps.agentScript.AddReward(-1.0f);// + -1 * ps.agentScript.timePenalty);//scored on
            }
            ps.agentScript.EndEpisode();  //all agents need to be reset
        }
        outOfBounds();
    }

    // Update is called once per frame
    void Update()
    {
        if(ball.transform.localPosition.y <= 1 && lastPlayerWithBall != null)
        {
            lastPlayerWithBall.AddReward(-0.1f);
            lastPlayerWithBall = null; 
        }
        if (ball.transform.localPosition.z > 12 || ball.transform.localPosition.z < -12) //check if the ball is out of bounds
        {
            outOfBounds();
        }
        if (ball.transform.localPosition.y >= 25 || ball.transform.localPosition.y <= -2)
        {
            outOfBounds();
        }
        if (ball.transform.localPosition.x > 33 || ball.transform.localPosition.x < -33)
        {
            outOfBounds();
        }
    }
}
