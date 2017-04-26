using UnityEngine;
using System;
using UniRx;

public class player : MonoBehaviour
{
    CharacterController m_chara;
    RopeSimulate ropeSimulate;
    checkGround check;

    [Header("移動速度")]
    public float speed;
    private float speedTemp;
    [Header("移動時の下方向補正")]
    public float uPower = -0.5f;
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

    public Camera CameraBox;
    public GameObject Rope;
    private GameObject CopyRope = null;
    private GameObject originRope = null;
    private GameObject tailRope = null;
    [Header("(テスト用)ロープ先端の位置")]
    public GameObject target;

    Vector3 moveDirection = Vector3.zero;
    //Vector3 NormalizeDirection;
    //Vector3 Center;
    Vector3 Accel;
    //Vector3 correction;
    RaycastHit hit;
    bool isJump;
    float timer;

    void Start()
    {
        m_chara = GetComponent<CharacterController>();
        check = GetComponent<checkGround>();
        jumpTemp = jumpPower;
        speedTemp = speed;
        isJump = false;
        //Center = new Vector3(Screen.width / 2, Screen.height / 2, 0); 
        // correction = new Vector3(0, direction, 0);

    }
    void Update()
    {

        if (CopyRope == null)
        {
            //地面に接している時
            if (check.IsGrounded)
            {
                isJump = false;
                jumpTimer = 0.0f;
                //カメラの向きに移動
                float dx = Input.GetAxis("Horizontal");
                float dz = Input.GetAxis("Vertical");
                moveDirection = Quaternion.Euler(0, CameraBox.transform.localEulerAngles.y, 0) *
                                new Vector3(dx, uPower, dz);
                moveDirection = transform.TransformDirection(moveDirection);

                //向いてる方向ベクトルの正規化
                //NormalizeDirection = moveDirection.normalized;

                //移動速度処理
                //加速力ある時は加速
                moveDirection *= speed;

                //坂道判定
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    //坂の角度
                    //float angle = Vector3.Angle(hit.normal, Vector3.up);
                    Accel = Vector3.ProjectOnPlane(moveDirection.normalized, hit.normal);

                    //坂道での速度処理
                    //下り坂 + 移動入力有(加速)
                    if (Accel.y <= -0.1f && (dx != 0 || dz != 0))
                    {
                        AccelAdd(2);
                    }
                    //上り坂 or 平面 + 移動入力有
                    else if (speed >= speedTemp && (dx != 0 || dz != 0))
                    {
                        speed -= (Time.deltaTime * 5);
                    }
                    //上り坂 or 平面 + 移動入力無
                    else if (speed >= speedTemp && (dx == 0 && dz == 0))
                    {
                        speed = speedTemp;
                    }
                }

            }
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

            moveDirection.y -= gravity * Time.deltaTime;
            m_chara.Move(moveDirection * Time.deltaTime);

            //テスト用
            //targetオブジェクトと自身の位置でロープを生成
            if (Input.GetKeyDown(KeyCode.F))
            {
                CopyRope = Instantiate(Rope, target.transform.position, Quaternion.identity);
                ropeSimulate = CopyRope.GetComponent<RopeSimulate>();

                //初期化           引数(origin,tail) 
                ropeSimulate.Initialize(target.transform.position, transform.position);
                //ropeSimulate.SimulationStop();
            }
        }
        else
        {
            //ロープ削除
            if (Input.GetKeyDown(KeyCode.J))
            {
                Destroy(CopyRope);
            }

            //ロープの基準点とtargetを同期
            //ropeSimulate.originPosition = target.transform.position;
            CopyRope.transform.position = target.transform.position;

            //ロープの末尾と位置を同期
            transform.position = ropeSimulate.tailPosition;
        }
    }

    //加速度変更
    public void AccelAdd(int value)
    {
        speed += (value * Time.deltaTime * 3);
    }
}
