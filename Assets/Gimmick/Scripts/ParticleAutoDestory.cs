using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleAutoDestory : MonoBehaviour
{
    public float destoryTime;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(destoryTime);
        Destroy(gameObject);
    }
}
