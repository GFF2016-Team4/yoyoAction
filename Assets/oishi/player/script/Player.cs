using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CheckGround), typeof(CharacterController))]
class Player : MonoBehaviour
{
    CheckGround checkGround;
    CharacterController characterController;

    public GameObject a;
    [Header("加速(回転)")]
    public float RotateAcceleration     = 2;
    [Header("加速(地面)")]
    public float GroundAcceleration     = 3;
    [Header("加速(下り坂)")]
    public float DownhillAcceleration   = 1;
    [Header("減速(停止時)")]
    public float GroundDeceleration     = 2;
    [Header("減速(上り坂)")]
    public float UphillDeceleration     = 1;
    [Header("ジャンプ力")]
    public float jumpPower              = 8;
    [Header("重力")]
    public float gravity                = 20;
    [Header("ロープを伸ばせる距離")]
    public float ropeDistance           = 33;
    [Header("最高速度")]
    public float maxSpeed               = 10;
    [SerializeField, Header("現在の速度")]
    float nowPlayerSpeed;
    [Header("ブレーキの強さ")]
    public float brakePower             = 2.0f;

    [Header("TPS視点用カメラ")]
    public Camera tpsCamera;

    public LayerMask layerMask;

    public GameObject bulletPrefab;

    Vector2 ScreenCenter;

    Vector3 moveDirection;

    float nowGravityPower;

    Vector2 inputVelocity;
    Vector2 previousDir = new Vector2(2, 1);
    Vector2 judgeDir;
    private RaycastHit hitShot;

    private GameObject bulletInst = null;

    bool isRotate;      //回転中かどうか
    bool isBrake;       //ブレーキ中かどうか
    bool isSrant;       //斜め移動してるかどうか
    float brakeSpeed;

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
        ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    void Update()
    {
        if (checkGround.IsGrounded)
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }

        nowGravityPower -= gravity * Time.deltaTime;
        Vector3 translate = moveDirection * nowPlayerSpeed;
        translate.y = nowGravityPower;
        characterController.Move(translate * Time.deltaTime);
    }

    void GroundMove()
    {
        //重力をリセット
        nowGravityPower = 0;

        InputExtension.GetAxisVelocityRaw(out inputVelocity);

        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit);
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
                Debug.Log("下り坂");
            }

            //平面 + 移動中(加速)
            if (Mathf.Abs(groundAngle.y) <= 0.1f)
            {
                AccelAdd(GroundAcceleration);
                Debug.Log("平面");
            }

            judgeDir = inputVelocity + previousDir;

            judgeDir = Vector3.Normalize(judgeDir);
            //反対のキーを押した時
            if (judgeDir.magnitude == 0 || (isSrant == true && judgeDir.magnitude == 0))
            {
                Debug.Log("反対方向キー");
                isBrake = true;
                brakeSpeed = nowPlayerSpeed * brakePower;
            }
            else if (judgeDir.magnitude < 1)
            {
                Debug.Log("斜め移動キー");
                isSrant = true;
            }
            else
            {
                Debug.Log("同じ方向キー");
                isSrant = false;
            }
            moveDirection = forward * previousDir.y + right * previousDir.x;
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
                    isSrant = false;
                    Debug.Log("ブレーキ中止");
                }
                AccelAdd(-brakeSpeed);
            }
            if (nowPlayerSpeed <= 0)
            {
                isBrake = false;
                isSrant = false;
                judgeDir = Vector3.zero;
            }
        }

        //上り坂 + 移動中(減速)
        if (groundAngle.y >= 0.1f)
        {
            AccelAdd(-UphillDeceleration);
            Debug.Log("上り坂");
        }

        //nowPlayerSpeed = Mathf.Max(nowPlayerSpeed, 0);
        nowPlayerSpeed = Mathf.Clamp(nowPlayerSpeed, 0, maxSpeed);


        //スペースキーでジャンプ
        if (Input.GetButton("Jump"))
        {
            nowGravityPower += jumpPower;
        }

        if (Input.GetMouseButtonDown(0))
        {
            ShootBullet();
        }

    }

    void AirMove()
    {
        //スペースキーを離す、上昇中
        if (Input.GetButtonUp("Jump") && nowGravityPower > 0)
        {
            nowGravityPower *= 0.8f;
        }
    }

    void ShootBullet()
    {
        if (bulletInst != null) return;

        Ray ray = Camera.main.ScreenPointToRay(ScreenCenter);
        if (Physics.Raycast(ray, out hitShot, ropeDistance, layerMask))
        {
            //弾の生成
            Debug.Log(hitShot.collider.name);
            bulletInst = Instantiate(bulletPrefab, transform.position, tpsCamera.transform.rotation);
        }
        Debug.DrawRay(ray.origin, Camera.main.transform.forward * 100, Color.red);
    }

    /// <summary>
    /// オブジェクトを軸に回転
    /// </summary>
    /// <param name="target">回転軸となるオブジェクト</param>
    /// <param name="rotateDistance">回転を開始するオブジェクトとの距離</param>
    public void TurnAround(Transform target, float rotateDistance)
    {
        Vector3 p1 = transform.position - target.position;
        Vector3 p2 = transform.right;

        p1.y = 0;
        p2.y = 0;

        Vector3 P2 = Vector3.Normalize(p2);

        float distance = Vector3.Dot(p1, P2);
        float absDistance = Mathf.Abs(distance);

        if (p1.magnitude <= rotateDistance && isRotate == false)
        {
            isRotate = true;
            AccelAdd(RotateAcceleration);
        }
        if (isRotate == true)
        {
            moveDirection = Vector3.zero;
            Quaternion rotation = Quaternion.LookRotation(p1);
            transform.rotation = rotation;
            transform.RotateAround(target.position, Vector3.up, nowPlayerSpeed);
        }
        else
        {
            moveDirection = transform.right;
            AccelAdd(RotateAcceleration);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Wall")
        {
            nowPlayerSpeed = 0;
        }
    }

    public void AccelAdd(float value)
    {
        nowPlayerSpeed += value * Time.deltaTime;
    }
    public Vector3 HitPoint
    {
        get { return hitShot.point; }
    }
    public Vector3 HitPosition
    {
        get { return hitShot.transform.position; }
    }
    public Vector3 Position
    {
        get { return transform.position; }
    }
}