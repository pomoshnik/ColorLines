using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    private bool start = true;

    void Update()
    {
        if ((transform.position.z < -3) && (!start))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -3);
        }
    }
}
