using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class StageGoal : MonoBehaviour
{
    [SerializeField]
    UnityEvent goalEvent;

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //ゴール
            Debug.Log("goal");
            goalEvent.Invoke();
        }
    }
}
