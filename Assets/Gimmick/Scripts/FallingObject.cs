using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
public class FallingObject : MonoBehaviour
{
    public GameObject particle;

    public float speed;
    Material mat;
    
    Color matColor;

    void Awake()
    {
        var renderer = GetComponent<Renderer>();
        mat = renderer.materials[0];
        mat.EnableKeyword("_EMISSION");
        matColor = mat.GetColor("_EmissionColor");
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Flash());
        }
    }

    IEnumerator Flash()
    {
        for (float i = 0.2f; i < 1.0; i+=Time.deltaTime*speed)
        {
            mat.SetColor("_EmissionColor", matColor * i * 5);
            yield return null;
        }
        mat.SetColor("_EmissionColor", matColor * 5);

        Instantiate(particle, transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(gameObject);
    }
}
