using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class checkGround : MonoBehaviour
{
    [Header("isGround判定の変更を何フレーム固定するか")]
    public int rockFrameTime = 5;

    private bool _isGrounded;
    public bool IsGrounded { get { return _isGrounded; } }

    // Use this for initialization
    void Start()
    {
        //isGroundの値が変化してからrockFrameTime以内の変更を無視
        var check = GetComponent<CharacterController>();

        check.ObserveEveryValueChanged(x => x.isGrounded)
               .ThrottleFrame(rockFrameTime)
               .Subscribe(x =>
               {
                   _isGrounded = x;
                   Debug.Log("change");
               });
    }
}
