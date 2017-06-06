using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CheckGround), typeof(CharacterController))]
class Player : MonoBehaviour
{
    CheckGround checkGround;
    CharacterController characterController;
    YoyoController yoyoController;
    playerCamera playerCamera;

    [Header("加速(回転離脱時)")]
    public float RotateAcceleration = 2;
    [Header("加速(地面)")]
    public float GroundAcceleration = 3;
    [Header("加速(下り坂)")]
    public float DownhillAcceleration = 1;
    [Header("減速(停止時)")]
    public float GroundDeceleration = 2;
    [Header("減速(上り坂)")]
    public float UphillDeceleration = 1;
    [Header("ジャンプ力")]
    public float jumpPower = 8;
    [Header("重力")]
    public float gravity = 20;
    [Header("ロープを伸ばせる距離")]
    public float ropeDistance = 33;
    [Header("最高速度")]
    public float maxSpeed = 10;
    [SerializeField, Header("現在の速度")]
    float nowPlayerSpeed;
    [Header("ブレーキの強さ")]
    public float brakePower = 2.0f;
    [Header("回転速度")]
    public float rotatePower = 20f;

    [Header("TPS視点用カメラ")]
    public Camera tpsCamera;

    public LayerMask layerMask;

    public GameObject bulletPrefab;

    Vector2 ViewportCenter;

    Vector3 moveDirection;

    float nowGravityPower;

    Vector2 inputVelocity;
    Vector2 previousDir = new Vector2(2, 1);
    Vector2 judgeDir;

    Vector3 lookRotation;
    Vector3 previousLook;

    private RaycastHit hitShot;
    private RaycastHit hit;

    private GameObject bulletInst = null;

    //public bool isRotate;      //回転中かどうか
    bool isBrake;       //ブレーキ中かどうか
    float brakeSpeed;
    float angle;
    bool judgeDirection;

    //インスペクター上でリセットの項目を選択したときの処理
    private void Reset()
    {
        playerCamera pCamera = GameObject.FindObjectOfType<playerCamera>();

        // ? がついているがこれはタイプミスではない（C#6の機能）
        tpsCamera = pCamera?.GetComponent<Camera>();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        checkGround = GetComponent<CheckGround>();
        playerCamera = GetComponent<playerCamera>();
        ViewportCenter = new Vector2(0.5f, 0.5f);
        previousLook = transform.position;
    }

    void Update()
    {
        //地面にいる時
        if (checkGround.IsGrounded/* && isRotate == false*/)
        {
            GroundMove();
        }
        //空中にいる時
        else
        {
            AirMove();
            transform.parent = null;
        }
        if (Input.GetMouseButtonDown(0))
        {
            ShootBullet();
        }
        if (bulletInst != null)
        {
            //ヨーヨーの打ち付けが終了している時
            if (yoyoController.NowBullet())
            {
                ResetGravity();
                if (hitShot.transform.tag == "Pillar")
                {
                    TurnAround(hitShot.point);
                }
            }
            //ヨーヨーを離した時
            else if (!yoyoController.NowBullet() && judgeDirection == true)
            {
                //最後に進んでる方向にプレイヤーを飛ばす
                judgeDirection = false;

                //if (nowPlayerSpeed <= 0.1f)
                //{
                //    nowPlayerSpeed = 1.0f;
                //}
                //moveDirection = transform.right;
                //AccelAdd(RotateAcceleration);
                //transform.rotation = Quaternion.identity;
                //isRotate = false;
            }
        }
        //else
        //{
        //    isRotate = false;
        //}

        nowGravityPower -= gravity * Time.deltaTime;
        nowPlayerSpeed = Mathf.Clamp(nowPlayerSpeed, 0, maxSpeed);

        Vector3 translate = moveDirection * nowPlayerSpeed;
        translate.y = nowGravityPower;
        characterController.Move(translate * Time.deltaTime);

        //移動方向にｚ軸を向ける
        Vector3 euler;
        lookRotation = transform.position - previousLook;
        if (lookRotation.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(lookRotation);
            //変な方向向くため0に
            euler = transform.eulerAngles;
            euler.x = 0;
            euler.z = 0;

            transform.eulerAngles = euler;

        }
        previousLook = transform.position;
    }

    void GroundMove()
    {
        //重力をリセット
        ResetGravity();

        InputExtension.GetAxisVelocityRaw(out inputVelocity);

        Physics.Raycast(transform.position, Vector3.down, out hit);
        Vector3 groundAngle = Vector3.ProjectOnPlane(moveDirection.normalized, hit.normal);

        if (inputVelocity.magnitude >= 0.8f && isBrake == false)
        {
            Vector3 forward = tpsCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = tpsCamera.transform.right;
            right.y = 0;
            right.Normalize();

            //moveDirection = Quaternion.Euler(0, tpsCamera.transform.localEulerAngles.y, 0) * inputVelocity.normalized;
            //moveDirection = transform.TransformDirection(moveDirection);
            //坂道判定
            //坂の角度
            //坂道での速度処理
            //下り坂 + 移動中(加速)
            if (groundAngle.y <= -0.1f)
            {
                AccelAdd(DownhillAcceleration);
            }

            //平面 + 移動中(加速)
            if (Mathf.Abs(groundAngle.y) <= 0.1f)
            {
                AccelAdd(GroundAcceleration);
            }

            judgeDir = inputVelocity + previousDir;

            judgeDir = Vector3.Normalize(judgeDir);
            //反対のキーを押した時
            if (judgeDir.magnitude == 0)
            {
                isBrake = true;
                brakeSpeed = nowPlayerSpeed * brakePower;
            }

            moveDirection = forward * previousDir.y + right * previousDir.x;
            //MoveDirection = forward * GetInputVelocity.y + right * GetInputVelocity.x;
            previousDir = inputVelocity;
        }

        else
        {
            AccelAdd(-GroundDeceleration);

            //ブレーキ中
            if (isBrake == true)
            {
                //ブレーキ中に他のキーが押されたら中止
                if (inputVelocity != previousDir)
                {
                    isBrake = false;
                }
                AccelAdd(-brakeSpeed);
            }
            if (nowPlayerSpeed <= 0)
            {
                isBrake = false;
                judgeDir = Vector3.zero;
            }
        }

        //上り坂 + 移動中(減速)
        if (groundAngle.y >= 0.1f)
        {
            AccelAdd(-UphillDeceleration);
        }

        //nowPlayerSpeed = Mathf.Max(nowPlayerSpeed, 0);
        //nowPlayerSpeed = Mathf.Clamp(nowPlayerSpeed, 0, maxSpeed);


        //スペースキーでジャンプ
        if (Input.GetButton("Jump"))
        {
            nowGravityPower += jumpPower;
        }
    }

    void AirMove()
    {

        InputExtension.GetAxisVelocityRaw(out inputVelocity);
        if (inputVelocity.magnitude >= 0.8f)
        {

            Vector3 forward = tpsCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = tpsCamera.transform.right;
            right.y = 0;
            right.Normalize();

            moveDirection = forward * previousDir.y + right * previousDir.x;
            previousDir = inputVelocity;
        }
        //スペースキーを離す、上昇中
        if (Input.GetButtonUp("Jump") && nowGravityPower > 0)
        {
            nowGravityPower *= 0.8f;
        }
    }

    void ShootBullet()
    {
        if (bulletInst != null) return;

        Ray ray = Camera.main.ViewportPointToRay(ViewportCenter);
        if (Physics.Raycast(ray, out hitShot, ropeDistance, layerMask))
        {
            if (hitShot.collider.tag == "Rail")
            {
                hitShot.collider.GetComponent<RailController>().enabled = true;
            }
            //弾の生成
            Quaternion rot = transform.rotation;
            float offset = tpsCamera.transform.GetComponent<playerCamera>().GetCameraRotate().x;
            rot.eulerAngles = new Vector3(rot.eulerAngles.x + 90 - offset, rot.eulerAngles.y, rot.eulerAngles.z);
            bulletInst = Instantiate(bulletPrefab, transform.position, rot);
            yoyoController = bulletInst.GetComponent<YoyoController>();
        }
    }

    /// <summary>
    /// オブジェクトを軸に回転
    /// </summary>
    /// <param name="target">回転軸となるオブジェクト</param>
    public void TurnAround(Vector3 target)
    {
        if (judgeDirection == false)
        {
            angle = Vector3.Angle(transform.position - target, transform.right);

            Vector3 p1 = transform.position - target;
            Vector3 p2 = transform.right;

            p1.y = 0;
            p2.y = 0;

            float distance = Vector3.Dot(p1, p2);
            float absDistance = Mathf.Abs(distance);

            yoyoController.ropeSimulate.ReCalcDistance(absDistance);
            judgeDirection = true;
        }

        //target.y = transform.position.y;
        Quaternion lookRotation = Quaternion.LookRotation(yoyoController.ropeSimulate.direction);
        Vector3 circleDir;
        if (angle <= 90)
        {
            circleDir = Vector3.right;
        }
        else
        {
            circleDir = -Vector3.right;
        }
        yoyoController.ropeSimulate.AddForce(lookRotation * circleDir * rotatePower, ForceMode.Force);


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
        if (colHit.transform.tag == "Wall" && hit.transform.tag != "Wall")
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
    public void AccelAdd(float value)
    {
        PlayerSpeed += value * Time.deltaTime;
    }
    public void ResetGravity()
    {
        nowGravityPower = 0;
    }
    public void SideMove()
    {
        InputExtension.GetAxisVelocityRaw(out inputVelocity);

        Vector3 forward = tpsCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = tpsCamera.transform.right;
        right.y = 0;
        right.Normalize();

        if (inputVelocity.magnitude != 0)
        {
            MoveDirection = forward * GetInputVelocity.y + right * GetInputVelocity.x;
            //MoveDirection = right * GetInputVelocity.y;
        }
    }
    public Vector3 HitPoint
    {
        get { return hitShot.point; }
    }
    public Vector3 HitPosition
    {
        get { return hitShot.transform.position; }
    }
    public RaycastHit hitInfo
    {
        get { return hitShot; }
    }
    public Vector3 Position
    {
        get { return transform.position; }
    }
    public Vector3 GetInputVelocity
    {
        get { return inputVelocity; }
    }
    public Vector3 MoveDirection
    {
        get { return moveDirection; }
        set { moveDirection = value; }
    }
    public float PlayerSpeed
    {
        get { return nowPlayerSpeed; }
        set { nowPlayerSpeed = value; }
    }
    public bool PlayerIsGround
    {
        get { return checkGround.IsGrounded; }
    }
}