using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testBall : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform ball;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(-ball.localPosition.x, ball.localPosition.y, -ball.localPosition.z);
        transform.localPosition = pos;
    }
}
