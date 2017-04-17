using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class reru : MonoBehaviour
{
    public float speed = 100;
    public Transform point;
    public Transform point2;
    public GameObject P;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = point.position;
            agent.speed = speed;

            P.transform.position = gameObject.transform.position;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = point2.position;
            agent.speed = speed;
        }
    }
}
