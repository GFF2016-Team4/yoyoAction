using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeCamera : MonoBehaviour
{
    public Camera CameraBox;
    public Transform PcameraPosition;

    [System.NonSerialized]
    public bool isMove = false;
    [Header("値が小さいほど移動速度が速くなる")]
    public float cameraSpeed = 1.0f;

    Vector3 movePos;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && isMove == false)
        {
            MoveToCameraBoxPosition_(transform.position, CameraBox.transform.position);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && isMove == false)
        {
            MoveToPCameraPosition_(transform.position, PcameraPosition.transform.position);
        }
    }

    //----------------------------同じ処理っぽいの２つ書いてるので要修正---------------------------------------------------------
    //TPS視点変更
    public Coroutine MoveToCameraBoxPosition_(Vector3 current, Vector3 target)
    {
        return StartCoroutine(MoveToCameraBoxPosition(current, target));
    }
    //肩越し視点変更
    public Coroutine MoveToPCameraPosition_(Vector3 current, Vector3 target)
    {
        return StartCoroutine(MoveToPcameraPosition(current, target));
    }


    //TPS視点変更処理
    IEnumerator MoveToCameraBoxPosition(Vector3 current, Vector3 target)
    {
        isMove = true;

        for (float i = 0.0f; i <= cameraSpeed; i += Time.deltaTime)
        {
            float t = i / cameraSpeed;

            movePos = getCameraBoxPosition;
            transform.position = Vector3.Lerp(current, movePos, t);
            yield return null;
        }
        transform.position = movePos;
        isMove = false;
    }

    //TPS視点変更処理
    IEnumerator MoveToPcameraPosition(Vector3 current, Vector3 target)
    {
        isMove = true;

        for (float i = 0.0f; i <= cameraSpeed; i += Time.deltaTime)
        {
            float t = i / cameraSpeed;

            movePos = getPcameraPosition;
            transform.position = Vector3.Lerp(current, movePos, t);
            yield return null;
        }
        transform.position = movePos;
        isMove = false;
    }
    //----------------------------同じ処理っぽいの２つ書いてるので要修正---------------------------------------------------------

    //TPS視点カメラのポジション取得
    Vector3 getCameraBoxPosition
    {
        get { return transform.parent.position; }
    }
    //肩越し視点カメラのポジション取得
    Vector3 getPcameraPosition
    {
        get { return PcameraPosition.transform.position; }
    }

}
