using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class playerCameraInfo : MonoBehaviour
{
    [SerializeField]
    private Camera playerCamera;

    public Vector3 position
    {
        get { return playerCamera.transform.position; }
    }

    public new Transform transform
    {
        get { return playerCamera.transform; }
    }

    public Vector3 forward
    {
        get { return transform.forward; }
    }

    public Vector3 right
    {
        get { return transform.right; }
    }

    public Vector3 up
    {
        get { return transform.up; }
    }

    //Item1:forward, Item2:right
    public Tuple<Vector3, Vector3> GetCameraAxis()
    {
        Vector3 right = playerCamera.transform.right;
        Vector3 forward = playerCamera.transform.forward;

        //変な方向に動くため
        right.y = 0;
        forward.y = 0;

        right.Normalize();
        forward.Normalize();

        return new Tuple<Vector3, Vector3>(forward, right);
    }

    public Vector3 GetInputVelocity()
    {
        var axis = GetCameraAxis();

        Vector3 velocity;
        velocity = axis.Item1 * Input.GetAxis("Vertical");
        velocity += axis.Item2 * Input.GetAxis("Horizontal");
        return velocity; //正規化はしない
    }
}