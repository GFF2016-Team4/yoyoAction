using UnityEngine;

[RequireComponent(typeof(CheckGround), typeof(CharacterController))]
class Player : MonoBehaviour
{
    CheckGround checkGround;
    CharacterController characterController;

    public GameObject a;
    [Header("加速(回転)")]
    public float RotateAcceleration;
    [Header("加速(地面)")]
    public float GroundAcceleration;
    [Header("加速(下り坂)")]
    public float DownhillAcceleration;
    [Header("減速(停止時)")]
    public float GroundDeceleration;
    [Header("減速(上り坂)")]
    public float UphillDeceleration;
    [Header("ジャンプ力")]
    public float jumpPower;
    [Header("重力")]
    public float gravity;
    [Header("ロープを伸ばせる距離")]
    public float ropeDistance;

    [Header("TPS視点用カメラ")]
    public Camera tpsCamera;

    public LayerMask layerMask;

    public GameObject bulletPrefab;

    Vector2 ScreenCenter;

    float nowPlayerSpeed;
    Vector3 moveDirection;

    float nowGravityPower;

    Vector2 inputVelocity;

    private RaycastHit hitShot;

    private GameObject bulletInst = null;

    bool isRotate;

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

        if (inputVelocity.magnitude >= 0.8f)
        {
            Vector3 forward = tpsCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = tpsCamera.transform.right;
            right.y = 0;
            right.Normalize();

            moveDirection = forward * inputVelocity.y + right * inputVelocity.x;

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
        }
        else
        {
            AccelAdd(-GroundDeceleration);
        }

        //上り坂 + 移動中(減速)
        if (groundAngle.y >= 0.1f)
        {
            AccelAdd(-UphillDeceleration);
            Debug.Log("上り坂");
        }

        nowPlayerSpeed = Mathf.Max(nowPlayerSpeed, 0);


        //スペースキーでジャンプ
        if (Input.GetButton("Jump"))
        {
            nowGravityPower += jumpPower;
        }

        if (Input.GetMouseButtonDown(1))
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