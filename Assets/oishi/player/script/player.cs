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
    [Header("移動時の下方向補正")]
    public float uPower = -0.5f;
    [Header("ジャンプ力")]
    public float jumpPower;
    [Header("重力")]
    public float gravity;
    //public float direction = 1.0f;

    public Camera Pcamera;

    public GameObject Rope;
    private GameObject CopyRope = null;
    private GameObject originRope = null;
    private GameObject tailRope = null;
    [Header("(テスト用)ロープ先端の位置")]
    public GameObject target;

    Vector3 moveDirection = Vector3.zero;
    Vector3 NormalizeDirection;
    Vector3 Center;
    RaycastHit hit;


    void Start()
    {
        m_chara = GetComponent<CharacterController>();
        check = GetComponent<checkGround>();
        //Center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    void Update()
    {
        if (CopyRope == null)
        {
            //地面に接している時
            if (check.IsGrounded)
            {
                //カメラの向きに移動
                moveDirection = Quaternion.Euler(0, Pcamera.transform.localEulerAngles.y, 0) * 
                                new Vector3(Input.GetAxis("Horizontal"), -0.5f, Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);

                //向いてる方向ベクトルの正規化
                NormalizeDirection = moveDirection.normalized;

                moveDirection *= speed;

                //スペースキーでジャンプ
                if (Input.GetButton("Jump")) moveDirection.y = jumpPower;
            }
            moveDirection.y -= gravity * Time.deltaTime;
            m_chara.Move(moveDirection * Time.deltaTime);

            //テスト用
            //targetオブジェクトと自身の位置でロープを生成
            if (Input.GetKeyDown(KeyCode.F))
            {
                CopyRope = Instantiate(Rope, transform.position, Quaternion.identity);

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

            //ロープの末尾と位置を同期
            transform.position = ropeSimulate.tailPosition;
        }
    }
}