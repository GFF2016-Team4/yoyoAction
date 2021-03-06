﻿using UnityEngine;
using System;
using UniRx;

public class playerCamera : MonoBehaviour
{
    [Tooltip("注目するオブジェクト")]
    public Transform target;

    [Tooltip("ターゲットとの距離")]
    public float distance = 1.0f;

    [Tooltip("オフセット")]
    public Vector3 offset;

    [Tooltip("回転スピード")]
    public Vector2 rotationSpeed = new Vector2(120.0f, 120.0f);

    [Tooltip("カメラの上下回転の限界")]
    public float cameraLimitUp = 30f;

    [Tooltip("カメラの上下回転の限界")]
    public float cameraLimitDown = -30f;

    CursorLockMode mode = CursorLockMode.None;

    private float variable;

    public float mystery;

    [Header("空のオブジェクト")]
    public Transform empty;
    public float smoothing = 10f;

    [Header("Playerとの距離")]
    public float km = 3f;

    [Header("カメラの感度")]
    public float mouseSpeed = 3f;

    Vector2 rotate;
    public void Start()
    {
        LockCursor();

        transform.forward = target.forward;
        transform.position = transform.forward * distance + offset;
    }

    void LateUpdate()
    {
        ChangeCursorState();

        
        rotate.x = Input.GetAxis("Horizontal2") * -rotationSpeed.x * Time.deltaTime / mouseSpeed;
        rotate.y = Input.GetAxis("Vertical2") * rotationSpeed.y * Time.deltaTime / mouseSpeed;

        //回転
        empty.RotateAround(target.position, Vector3.up, rotate.x);
        empty.RotateAround(target.position, transform.right, rotate.y);


        transform.position = Vector3.Lerp(
            transform.position, empty.position, smoothing * Time.deltaTime);

        transform.LookAt(target);

        transform.position = (
            transform.position - target.position).normalized * km + target.position;

        //FixedAngle();

        Ray ray = new Ray()
        {
            origin = target.position + offset,
            direction = -transform.forward
        };

        RaycastHit hitInfo;
        bool isHit = Physics.Raycast(ray, out hitInfo, distance, playersLayerMask.IgnorePlayerAndRopes);

        if (isHit)
        {
            transform.position = hitInfo.point;
        }
        else
        {
            Vector3 position = target.position;       //初期化
            position -= transform.forward * distance; //ターゲットの後ろに下がって見やすいように
            position += offset;                       //オフセット値

            //座標の変更
            //transform.position = position;
        }
    }

    void ChangeCursorState()
    {
        //EscapeだけだとEscapeを押したときに表示しっぱなしになる
        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnLockCursor();
        }

        Cursor.lockState = mode;
    }

    void LockCursor()
    {
        mode = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnLockCursor()
    {
        mode = CursorLockMode.None;
        Cursor.visible = true;
    }

    //void FixedAngle()
    //{
    //    Vector3 angle = transform.eulerAngles;

    //    if (Input.GetButtonDown("ResetCamera"))
    //    {
    //        angle.x = 0;
    //    }
    //    else if (angle.x >= 180)
    //    {
    //        //angleは取得時に0～360の値になるため
    //        angle.x -= 360;
    //    }
    //    //上限値・下限値を設定してカメラが変な挙動をしないように
    //    angle.x = Mathf.Clamp(angle.x, cameraLimitDown, cameraLimitUp);
    //    angle.z = 0;
    //    transform.eulerAngles = angle;
    //}
}