using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireSparks : MonoBehaviour
{
    [Header("大きいほど一度にでるパーティクルの量が変わる")]
    public float rate;
    public float randomRange;

    new ParticleSystem            particleSystem;
    ParticleSystem.EmissionModule module;
    Vector3 prevPos;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        module = particleSystem.emission;
    }

    void Start()
    {

    }

    void Update()
    {
        float speed = Vector3.Distance(transform.position, prevPos);

        module.rateOverDistance = Random.Range(speed*rate - randomRange, speed*rate + randomRange); 
        prevPos = transform.position;
    }
}
