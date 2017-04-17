using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class na : MonoBehaviour
{
    public Transform[] target;
    public GameObject goruoya;

    private NavMeshAgent agent;

    int PIndex = 0;
    int kodomo;

    void Awake()
    {
        goruoya = GameObject.Find("goal");
        kodomo = goruoya.transform.childCount;
        for (int i = 0; i < kodomo; i++)
        {
            target[i] = goruoya.transform.GetChild(i);
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        set();
    }

    void Update()
    {
        if (agent.remainingDistance < 0.5f)
        {
            set();
        }

    }

    void set()
    {
        if (target.Length == 0) return;
        agent.destination = target[PIndex].position;
        PIndex = (PIndex + 1) % target.Length;
    }

}
