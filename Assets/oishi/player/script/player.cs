using UnityEngine;
using System;

public class player : MonoBehaviour
{
    CharacterController m_chara;

    public float speed;
    public float jumpPower;
    public float gravity;

    public Camera Pcamera;

    public GameObject Rope;
    private GameObject CopyRope = null;
    private GameObject originRope = null;
    private GameObject tailRope = null;
    public GameObject target;

    public float direction = 1.0f;

    Vector3 moveDirection = Vector3.zero;
    Vector3 NormalizeDirection;
    Vector3 Center;
    Vector3 tempPos;

    RaycastHit hit;

    RopeSimulate ropeSimulate;

    void Start()
    {
        m_chara = GetComponent<CharacterController>();
        ropeSimulate = GetComponent<RopeSimulate>();
        Center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    void Update()
    {
        Debug.Log("CopyRope:" + CopyRope);
        //Debug.Log(originRope);
        //Debug.Log(tailRope);

        if (CopyRope == null)
        {
            //地面に接している時
            if (m_chara.isGrounded)
            {
                moveDirection = Quaternion.Euler(0, Pcamera.transform.localEulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);

                //向いてる方向ベクトルの正規化
                NormalizeDirection = moveDirection.normalized;

                moveDirection *= speed;

                if (Input.GetButton("Jump")) moveDirection.y = jumpPower;
            }
            moveDirection.y -= gravity * Time.deltaTime;
            m_chara.Move(moveDirection * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.G))
            {
                CopyRope = Instantiate(Rope, transform.position, Quaternion.identity);
                originRope = CopyRope.transform.FindChild("Origin").gameObject;
                tailRope = CopyRope.transform.FindChild("Tail").gameObject;

                ropeSimulate = CopyRope.GetComponent<RopeSimulate>();
                //初期化           引数(origin,tail) 
                ropeSimulate.Initialize(target.transform.position, m_chara.transform.position);
                //ropeSimulate.SimulationStop();
            }

        }
        else
        {
            m_chara.transform.position = tailRope.transform.position;
        }
    }
}