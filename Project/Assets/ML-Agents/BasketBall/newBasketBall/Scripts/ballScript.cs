using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class ballScript : MonoBehaviour
{
    public GameObject player;
    public gameController gc;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.transform.parent.GetChild(2).gameObject.transform.GetChild(0).gameObject; //get player if exists
    }

    void OnCollisionEnter(Collision col)//called when entering a collision
    {
        if (col.gameObject.CompareTag("target"))
        {
            //Debug.Log("BASKET!");
            if (player != null) //call funcs if basket is made
            {
                if (player.transform.Find("Shooting").GetComponent<shoot_nn_script>())
                    player.transform.Find("Shooting").GetComponent<shoot_nn_script>().basketMade();
                if (player.transform.Find("Shooting").GetComponent<PlayingShotNN>() && player.GetComponent<ShootingPlayerScript>())
                    player.transform.Find("Shooting").GetComponent<PlayingShotNN>().basketMade();
            }
            if (gc != null)
            {
                gc.basketMade();
            }
            return;
        }
        if (col.gameObject.CompareTag("purpleGoal"))
        {
            if (gc != null)
            {
                gc.TeamScored(Full_train_nn.Team.Red);
            }
            return;
        }
        if (col.gameObject.CompareTag("blueGoal"))
        {
            if (gc != null)
            {
                gc.TeamScored(Full_train_nn.Team.Blue);
            }
            return;
        }
        
        if (col.gameObject.CompareTag("Player"))
        {
            if (col.gameObject.GetComponent<Rigidbody>())
            {
                col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                col.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            if (player.transform.Find("Passing").GetComponent<pass_nn_script>())
                player.transform.Find("Passing").GetComponent<pass_nn_script>().passMade();
            //un comment if using playing pass nn
            if (player.transform.Find("Passing").GetComponent<PlayingPassNN>() && player.GetComponent<ShootingPlayerScript>())
                player.transform.Find("Passing").GetComponent<PlayingPassNN>().passMade();
            //if (player.GetComponent<pass_nn_script>())
            //    player.GetComponent<pass_nn_script>().passMade();
            return;
        }
    }
}
