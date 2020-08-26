using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPlayerScript : MonoBehaviour
{
    public gameController gc;
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("target"))
        {
            Debug.Log("BASKET!");
            gc.basketMade();
            return;
        }
    }
}
