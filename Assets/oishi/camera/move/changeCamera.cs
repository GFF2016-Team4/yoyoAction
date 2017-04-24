using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeCamera : MonoBehaviour
{
    public Camera CameraBox;
    public Transform PcameraPosition;

    [SerializeField]
    public bool isMove = false;
    [Header("値が小さいほど移動速度が速くなる")]
    public float cameraSpeed = 1.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && isMove == false)
        {
            Move_(transform.position, CameraBox.transform.position);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && isMove == false)
        {
            Move_(transform.position, PcameraPosition.transform.position);
        }
    }

    public Coroutine Move_(Vector3 current, Vector3 target)
    {
        return StartCoroutine(Move(current, target));
    }
    IEnumerator Move(Vector3 current, Vector3 target)
    {
        isMove = true;
        for (float i = 0.0f; i <= cameraSpeed; i += Time.deltaTime)
        {
            float t = i / cameraSpeed;
            transform.position = Vector3.Lerp(current, target, t);
            yield return null;
        }
        transform.position = target;
        isMove = false;
    }

}
