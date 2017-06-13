using UnityEngine;
using System.Collections;
using SlopeState = CheckGround.SlopeState;
using GripperState  = Gripper2.State;

public enum PlayerState
{
	NormalMove,
	RopeWait,
	TarzanMove,
	RailMove,
	CircleMove,
}

[RequireComponent(typeof(CheckGround), typeof(CharacterController))]
class Player : MonoBehaviour
{
	// ----- ----- ----- -----
	// Inspectorで編集可能
	// ----- ----- ----- -----

	[Header("加速(回転離脱時)")]
    public float RotateAcceleration   = 2;
    [Header("加速(地面)")]
    public float GroundAcceleration   = 3;
    [Header("加速(下り坂)")]
    public float DownhillAcceleration = 1;
    [Header("減速(停止時)")]
    public float GroundDeceleration   = 2;
    [Header("減速(上り坂)")]
    public float UphillDeceleration   = 1;
    [Header("ジャンプ力")]
    public float jumpPower            = 8;
    [Header("重力")]
    public float gravity              = 20;
    [Header("ロープを伸ばせる距離")]
    public float ropeDistance         = 33;
    [Header("最高速度")]
    public float maxSpeed             = 10;
    [Header("ブレーキの強さ")]
    public float brakePower           = 2.0f;
    [Header("回転速度")]
    public float rotatePower          = 20f;

	[SerializeField]
	private new Camera camera;

	[SerializeField]
    private LayerMask  layerMask;

	[SerializeField]
    private GameObject bulletPrefab;

	// ----- ----- ----- -----
	// Inspectorで編集不可
	// ----- ----- ----- -----

	CheckGround         checkGround;
	CharacterController characterController;
	Gripper2            gripperInst;
	playerCamera        playerCamera;

	Vector3 moveDirection;

	float   nowPlayerSpeed;
    float   nowGravityPower;

    Vector2 inputVelocity;
    Vector2 previousDir = new Vector2(2, 1);

    private RaycastHit hitShot;

    private GameObject bulletInst = null;

    //public bool isRotate;      //回転中かどうか
    float brakeSpeed;
    float angle;
    bool  judgeDirection;

	PlayerState state = PlayerState.NormalMove;

	// ----- ----- ----- -----
	// 定数(structはreadonlyで代用)
	// ----- ----- ----- -----

	const string JUMP_KEY = "Jump";
	const string WALL_TAG = "Wall";

	readonly Vector2 VIEW_POSITION_CENTER = new Vector2(0.5f, 0.5f);



	// ----- ---- -----
	//     プロパティ
	// ----- ---- -----

	public RaycastHit hitInfo          => hitShot;
	public Vector3    HitPoint         => hitShot.point;
	public Vector3    HitPosition      => hitShot.transform.position;

	public Vector3    Position         => transform.position;

	public Vector3    GetInputVelocity => inputVelocity;

	public bool       IsGround         => checkGround.IsGrounded;

	public Vector3 MoveDirection
	{
		get { return moveDirection; }
		set { moveDirection = value; }
	}

	public float MoveSpeed
	{
		get { return nowPlayerSpeed; }
		set { nowPlayerSpeed = value; }
	}

	// ----- ---- -----
	//       関数
	// ----- ---- -----

	//インスペクター上でリセットの項目を選択したときの処理
	private void Reset()
    {
        playerCamera pCamera = FindObjectOfType<playerCamera>();

        // ? がついているがこれはタイプミスではない（C#6の機能）
        camera = pCamera?.GetComponent<Camera>();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        checkGround         = GetComponent<CheckGround>();
        playerCamera        = camera.GetComponent<playerCamera>();
    }

    void Update()
    {
		switch (state)
		{
			case PlayerState.NormalMove: NormalMove(); break;
			case PlayerState.RopeWait:   RopeWait();   break;
			case PlayerState.TarzanMove: TarzanMove(); break;
			case PlayerState.RailMove:   RailMove();   break;
			case PlayerState.CircleMove:  break;

			default:
				Debug.LogError("無効な値です");
				break;
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (gripperInst == null) return;
			gripperInst.ChangeState(GripperState.TakeUp);
		}
    }

	void NormalMove()
	{
		InputExtension.GetAxisVelocityRaw(out inputVelocity);

		if (checkGround.IsGrounded)
		{
			GroundMove();
		}
		else
		{
			AirMove();
			//transform.parent = null;
		}

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = camera.ViewportPointToRay(VIEW_POSITION_CENTER);

			Vector3 shootDir = ray.direction;

			if (Physics.SphereCast(ray, 0.5f, out RaycastHit hitInfo, ropeDistance, layerMask))
			{
				shootDir = hitInfo.point - transform.position;
			}
			else
			{
				Vector3 endPoint = ray.direction.normalized * ropeDistance;
				endPoint += ray.origin;
				shootDir = endPoint      - transform.position;
			}

			Vector3 shootPos = transform.position;
			shootPos.y += 1.0f;

			gripperInst = Gripper2.Shoot(shootPos, shootDir, ropeDistance);

			gripperInst.callback = OnRopeHitEvent;

			Debug.Log("state change : RopeWait");
			state = PlayerState.RopeWait;
		}

		nowGravityPower -= gravity * Time.deltaTime;
		nowPlayerSpeed = Mathf.Clamp(nowPlayerSpeed, 0, maxSpeed);

		Vector3 translate = moveDirection * nowPlayerSpeed;
		translate.y = nowGravityPower;
		characterController.Move(translate * Time.deltaTime);

		if (!moveDirection.IsZero())
		{
			transform.rotation = Quaternion.LookRotation(moveDirection);
		}
	}

	void RopeWait()
	{
		if (gripperInst == null)
		{
			state = PlayerState.NormalMove;
			return;
		}

		InputExtension.GetAxisVelocityRaw(out inputVelocity);

		if (checkGround.IsGrounded)
		{
			GroundMove();
		}
		else
		{
			AirMove();
			//transform.parent = null;
		}

		nowGravityPower -= gravity * Time.deltaTime;
		nowPlayerSpeed = Mathf.Clamp(nowPlayerSpeed, 0, maxSpeed);

		Vector3 translate = moveDirection * nowPlayerSpeed;
		translate.y = nowGravityPower;
		characterController.Move(translate * Time.deltaTime);

		transform.rotation = Quaternion.LookRotation(moveDirection);

		gripperInst.ApplyTailPosition(transform.position);
	}

	void TarzanMove()
	{
		InputExtension.GetAxisVelocityRaw(out inputVelocity);

		bool isTarzan = nowGravityPower < 0;

		if (checkGround.IsGrounded)
		{
			GroundMove();
		}
		else
		{
			if (isTarzan)
			{
				//ターザン移動
				transform.position = gripperInst.FetchTailPosition();
				return;
			}
			else
			{
				AirMove();
			}
		}

		//ターザン移動中はプレイヤーはロープの動きに移動させるので通常の移動量計算をしない
		nowGravityPower -= gravity * Time.deltaTime;
		nowPlayerSpeed = Mathf.Clamp(nowPlayerSpeed, 0, maxSpeed);

		Vector3 translate = moveDirection * nowPlayerSpeed;
		translate.y = nowGravityPower;
		characterController.Move(translate * Time.deltaTime);

		transform.rotation = Quaternion.LookRotation(moveDirection);
	}

	void RailMove()
	{
		InputExtension.GetAxisVelocityRaw(out inputVelocity);
		bool isRailMove = nowGravityPower < 0;

		if (checkGround.IsGrounded)
		{
			GroundMove();
		}
		else
		{
			if (isRailMove)
			{
				//レール移動
				
			}
			else
			{
				AirMove();
			}
		}
	}

    void GroundMove()
    {
        //重力をリセット
        ResetGravity();

		SlopeState slopeState = checkGround.FetchSlope(transform.position, moveDirection);

		//上り坂は減速
		if (slopeState == SlopeState.UpHill)
		{
			Accel(-UphillDeceleration);
		}

		//スペースキーでジャンプ
		if (Input.GetButton(JUMP_KEY))
		{
			nowGravityPower += jumpPower;
		}

		if (IsBrake())
		{
			//-1 -> ブレーキ中では無い状態
			if (brakeSpeed == -1)
			{
				//ブレーキの初期化
				brakeSpeed = nowPlayerSpeed * brakePower;
			}

			Accel(-brakeSpeed);

			//ブレーキしている間は普通の移動を受け付けない
			return;
		}

		//ブレーキ解除後に１回だけ実行
		if(brakeSpeed != -1)
		{
			//ブレーキ中じゃないときは-1にする
			brakeSpeed = -1;
			previousDir = inputVelocity;
		}

		//普通の移動
		if (inputVelocity.magnitude >= 0.8f)
        {
			if (slopeState == SlopeState.DownHill)
            {
                Accel(DownhillAcceleration);
            }
            else if (slopeState == SlopeState.Flatten)
            {
                Accel(GroundAcceleration);
            }

			ApplyMoveDirection();
        }
		else
        {
			Accel(-GroundDeceleration);
        }
    }

    void AirMove()
    {
        if (inputVelocity.magnitude >= 0.8f)
        {
			ApplyMoveDirection();
		}
        //スペースキーを離す、上昇中
        if (Input.GetButtonUp(JUMP_KEY) && nowGravityPower > 0)
        {
            nowGravityPower *= 0.8f;
        }
    }

    /// <summary>
    /// オブジェクトを軸に回転
    /// </summary>
    /// <param name="target">回転軸となるオブジェクト</param>
    public void TurnAround(Vector3 target)
    {
        //if (judgeDirection == false)
        //{
        //    angle = Vector3.Angle(transform.position - target, transform.right);

        //    Vector3 p1 = transform.position - target;
        //    Vector3 p2 = transform.right;

        //    p1.y = 0;
        //    p2.y = 0;

        //    float distance = Vector3.Dot(p1, p2);
        //    float absDistance = Mathf.Abs(distance);

        //    yoyoController.ropeSimulate.ReCalcDistance(absDistance);
        //    judgeDirection = true;
        //}

        ////target.y = transform.position.y;
        //Quaternion lookRotation = Quaternion.LookRotation(yoyoController.ropeSimulate.direction);
        //Vector3 circleDir;
        //if (angle <= 90)
        //{
        //    circleDir = Vector3.right;
        //}
        //else
        //{
        //    circleDir = -Vector3.right;
        //}
        //yoyoController.ropeSimulate.AddForce(lookRotation * circleDir * rotatePower, ForceMode.Force);


        ////absDistanceに補正分追加したほうが落下しすぎない（？）
        ////必要なければ削除
        //if (p1.magnitude <= absDistance + rotationRadius && isRotate == false)
        //{
        //    isRotate = true;

        //    //AccelAdd(RotateAcceleration);
        //}
        //if (isRotate == true)
        //{
        //    moveDirection = Vector3.zero;
        //    Quaternion rotation = Quaternion.LookRotation(p1);
        //    transform.rotation = rotation;

        //    ResetGravity();

        //    if (angle <= 90)
        //    {
        //        transform.RotateAround(target, Vector3.down, nowPlayerSpeed);
        //    }
        //    else
        //    {
        //        transform.RotateAround(target, Vector3.up, nowPlayerSpeed);
        //    }
        //}
    }

    void OnControllerColliderHit(ControllerColliderHit colHit)
    {
        if (colHit.transform.tag == WALL_TAG && checkGround.groundInfo.tag != WALL_TAG)
        {
            nowPlayerSpeed = 0;
        }
        if (colHit.transform.tag == "MoveObject/Lift")
        {
            transform.parent = colHit.gameObject.transform;
            //transform.SetParent(colHit.gameObject.transform, true);
        }
        else
        {
            transform.parent = null;
        }
    }

	private void ApplyMoveDirection()
	{
		Vector3 forward = camera.transform.forward;
		forward.y = 0;
		forward.Normalize();

		Vector3 right = camera.transform.right;
		right.y = 0;
		right.Normalize();

		moveDirection = forward * previousDir.y + right * previousDir.x;
		previousDir = inputVelocity;
	}

    public void Accel(float value)
    {
        MoveSpeed += value * Time.deltaTime;
    }

	private bool IsBrake()
	{
		float angle = Vector2.Angle(previousDir, inputVelocity);
		if (nowPlayerSpeed <=   0) return false;
		if (angle          <  160) return false;
		return true;
	}

    public void ResetGravity()
    {
        nowGravityPower = 0;
    }

    public void SideMove()
    {
        InputExtension.GetAxisVelocityRaw(out inputVelocity);

        Vector3 forward = camera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = camera.transform.right;
        right.y = 0;
        right.Normalize();

        if (inputVelocity.magnitude != 0)
        {
            MoveDirection = forward * GetInputVelocity.y + right * GetInputVelocity.x;
            //MoveDirection = right * GetInputVelocity.y;
        }
    }

	private void OnRopeHitEvent(Collider hitInfo)
	{
		Debug.Log("hit");

		if (!Input.GetMouseButton(0))
		{
			gripperInst.ChangeState(GripperState.TakeUp);
			return;
		}

		if (hitInfo.tag == "Pillar")
		{
			//円運動

			Debug.Log("state change : CircleMove");
			state = PlayerState.CircleMove;
			gripperInst.ChangeState(GripperState.CircleMove);
			return;
		}

		if (hitInfo.tag == "Rail")
		{
			if (IsGround)
			{
				gripperInst.ChangeState(GripperState.NoSimulate);
			}

			//レールの進む向きは常にレールオブジェクトのforward方向で固定
			Vector3 reilMoveDir = hitInfo.transform.forward;
			Vector3 player2rail = hitInfo.transform.position - transform.position;

			//XZのみで判定
			reilMoveDir.y = 0;
			player2rail.y = 0;

			//帰ってくる値は0-180の間
			float angle = Vector3.Angle(reilMoveDir, player2rail);

			if (45 < angle && angle < 135)
			{
				Debug.Log("state change : TarzanMove");
				state = PlayerState.TarzanMove;
				if (IsGround)
				{
					gripperInst.ChangeState(GripperState.NoSimulate);
				}
				else
				{
					gripperInst.ChangeState(GripperState.TarzanMove);
				}
			}
			else
			{
				Debug.Log("state change : RailMove");
				state = PlayerState.RailMove;
				if (IsGround)
				{
					gripperInst.ChangeState(GripperState.NoSimulate);
				}
				else
				{
					gripperInst.ChangeState(GripperState.RailMove);
				}
			}
			return;
		}

		Debug.Log("Rope Takeup");
		gripperInst.ChangeState(GripperState.TakeUp);
	}
}