using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mawaru : MonoBehaviour
{
    public float rotation;
    public float speed = 0.1f;

    Vector3 angle;
    Rigidbody rb;
    // Use this for initialization
    void Start()
    {
        angle = new Vector3(rotation, 0, 0);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(transform.forward * speed);
        //transform.Rotate(Vector3.right,rotation);
        rb.angularVelocity = transform.forward * speed;
    }
}
