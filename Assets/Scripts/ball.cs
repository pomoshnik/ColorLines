using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    private bool start = true;
    void OnCollisionEnter()
    {
        if (start) {
            var comp = GetComponent<Rigidbody>();
            comp.velocity = Vector3.zero;
            comp.angularVelocity = Vector3.zero;
            comp.isKinematic = true;
            start = false;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Hit");
        var comp = GetComponent<Rigidbody>();
        comp.isKinematic = false;
        comp.AddForce(new Vector3(0, 0, -5), ForceMode.Impulse);
        
    }

    void Update()
    {
        if ((transform.position.z<-3)&&(!start))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -3);
        }
    }
}
