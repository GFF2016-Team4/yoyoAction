using System.Collections;
using UnityEngine;
using System;
using UniRx;

public class Player : MonoBehaviour
{
    CharacterController characterController;
    checkGround check;

    [Header("移動速度")]
    public float speed;
    [Header("加速(下り坂)")]
    public float DownhillAccel;
    [Header("加速(地面)")]
    public float GroundAccel;
    [Header("減速(上り坂)")]
    public float UphillAccel;
    [Header("減速(停止時)")]
    public float StopAccel;
    [Header("ジャンプ力")]
    public float jumpPower;
    [Header("ジャンプ時間")]
    public float jumpTime;
    private float jumpTemp;
    private float jumpTimer;
    [Header("重力")]
    public float gravity;
    [Header("ジャンプ慣性の力")]
    public float inertia;
    [Header("ロープを伸ばせる距離")]
    public float distance;

    [Header("TPS視点用カメラ")]
    public Camera CameraBox;
    [Header("肩越し視点用カメラ")]
    public Camera Pcamera;
    public GameObject Bullet;

    private GameObject CopyBullet = null;

    Vector3 moveDirection = Vector3.zero;
    Vector3 moveDirectionY = Vector3.zero;
    Vector3 Center;
    Vector3 groundAngle;

    RaycastHit hit;
    public RaycastHit hitShot;
    bool isJump;
    float timer;
    int layerMask;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        check = GetComponent<checkGround>();
        jumpTemp = jumpPower;
        isJump = false;
        Center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        layerMask = LayerMask.GetMask("Player");
        layerMask = ~layerMask;

    }
    void Update()
    {
        //地面に接している時
        if (check.IsGrounded)
        {
            isJump = false;
            jumpTimer = 0.0f;
            moveDirectionY.y = 0.0f;
            //カメラの向きに移動
            float dx = Input.GetAxisRaw("Horizontal");
            float dz = Input.GetAxisRaw("Vertical");

            Vector3 inputVelocity = new Vector3(dx, 0, dz);

            Physics.Raycast(transform.position, Vector3.down, out hit);
            groundAngle = Vector3.ProjectOnPlane(moveDirection.normalized, hit.normal);

            if (inputVelocity.magnitude >= 0.8f)
            {
                moveDirection = Quaternion.Euler(0, CameraBox.transform.localEulerAngles.y, 0) * inputVelocity.normalized;
                moveDirection = transform.TransformDirection(moveDirection);
                //坂道判定
                //坂の角度
                //float angle = Vector3.Angle(hit.normal, Vector3.up);

                //坂道での速度処理
                //下り坂 + 移動中(加速)
                if (groundAngle.y <= -0.1f)
                {
                    AccelAdd(DownhillAccel);
                    Debug.Log("下り坂");
                }
                //平面 + 移動中(加速)
                if (Mathf.Abs(groundAngle.y) <= 0.1f)
                {
                    AccelAdd(GroundAccel);
                    Debug.Log("平面");
                }
            }
            else AccelAdd(StopAccel);
            //上り坂 + 移動中(減速)
            if (groundAngle.y >= 0.1f)
            {
                AccelAdd(UphillAccel);
                Debug.Log("上り坂");
            }

            speed = Math.Max(speed, 0);
            //スペースキーでジャンプ
            if (Input.GetButton("Jump") && isJump == false && jumpTime >= jumpTimer)
            {
                jumpPower += Time.deltaTime;
                jumpTimer += Time.deltaTime;
                moveDirection.y = jumpPower;

                if (jumpTime <= jumpTimer)
                {
                    isJump = true;
                    jumpPower = jumpTemp;
                    moveDirection.y *= inertia;
                }
            }
            //スペースキーを離す、上昇中
            if (Input.GetButtonUp("Jump") && jumpTime >= jumpTimer && moveDirection.y >= 0.0f)
            {
                isJump = true;
                jumpPower = jumpTemp;
                moveDirection.y *= inertia;
            }

            //グリッパーを射出してない場合
            if (CopyBullet == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Center);
                var isHit = Physics.Raycast(ray, out hitShot, distance, layerMask);
                if (isHit)
                {
                    Debug.Log(hitShot.collider.name);
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        //弾の生成
                        CopyBullet = Instantiate(Bullet, transform.position, CameraBox.transform.rotation);
                    }
                }
                Debug.DrawRay(ray.origin, Camera.main.transform.forward * 100, Color.red);
            }

        }
        moveDirectionY.y -= gravity * Time.deltaTime;
        characterController.Move((moveDirection * speed + moveDirectionY) * Time.deltaTime);
    }
    //加速度変更
    public void AccelAdd(float value)
    {
        speed += (value * Time.deltaTime);
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
