using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CheckGround : MonoBehaviour
{
	public enum SlopeState
	{
		Flatten,
		DownHill,
		UpHill,
	}

	[Header("isGround判定の変更を何フレーム固定するか")]
    public int rockFrameTime = 5;

    private bool _isGrounded;
    public bool IsGrounded { get { return _isGrounded; } }

	Vector3 down = Vector3.down;

	RaycastHit groundHit;
	public GameObject groundInfo
	{
		get
		{
			return groundHit.collider.gameObject;
		}
	}

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
               });
    }

	void Update()
	{
		//地面の情報を更新
		Physics.Raycast(transform.position, down, out groundHit);
	}

	///<summary>指定した座標のスロープの情報を取得します</summary>
	public SlopeState FetchSlope(Vector3 origin, Vector3 moveDirection)
	{
		Vector3 groundAngle = Vector3.ProjectOnPlane(moveDirection.normalized, groundHit.normal);

		if (groundAngle.y >= 0.1f) { return SlopeState.UpHill;   } //上り坂
		if (groundAngle.y <= 0.1f) { return SlopeState.DownHill; } //下り坂

		return SlopeState.Flatten;                                  //平面
	}
}
