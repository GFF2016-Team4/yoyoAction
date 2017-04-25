using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoController : MonoBehaviour
{
    [SerializeField, Header("プレイヤーの手元")]
    public Transform m_HandOrigin;
    [SerializeField, Header("最小ヨーヨー間隔")]
    public float m_SpaceDisMin = 0.6f;
    [SerializeField, Header("最大ヨーヨー間隔")]
    public float m_SpaceDisMax = 3.0f;
    [SerializeField, Header("投げ出す距離")]
    public float m_ThrowDis;

    private float m_Space;                      //ヨーヨーの間隔
    private Vector3 m_ScalePrototype;           //ヨーヨーの大きさ(原型)
    private Vector3 m_PosPrototype;             //ヨーヨーの場所(原型)
    private Quaternion m_RotPrototype;          //ヨーヨーの向き(原型)

    private float m_ScaleNum = 10.0f;           //拡大倍率
    private float m_SeparationSpeed = 1.0f;     //分離速度

    private GameObject m_Left;                  //ヨーヨー左部分
    private GameObject m_Right;                 //ヨーヨー右部分

    private Collider m_TargetCollider;
    private Rigidbody m_Rigidbody;

    private bool m_IsOpened = false;            //開いたか?
    private bool m_IsCollised = false;          //当たったか?
    private bool m_IsHorizontal = false;        //水平であるか?
    private bool m_IsRailMoving = false;        //レール移動中であるか?

    // Use this for initialization
    void Start()
    {
        m_Left = transform.FindChild("Left").gameObject;
        m_Right = transform.FindChild("Right").gameObject;

        m_Rigidbody = transform.GetComponent<Rigidbody>();
        //手元の大きさと場所を取得
        GetPrototypeAttribute();
    }

    // Update is called once per frame
    void Update()
    {
        //間隔を計算
        m_Space = (float)((int)Vector3.Distance(m_Left.transform.position, m_Right.transform.position));

        //ヨーヨーの回転制御
        if (Input.GetKeyDown(KeyCode.Q))
        {
            this.transform.Rotate(Vector3.forward, 90);
            m_IsHorizontal = !m_IsHorizontal;
        }

        //ヨーヨー移動制御
        if (Input.GetMouseButton(1))
        {
            if (m_IsCollised == false)
            {
                m_Rigidbody.velocity = transform.forward * 10;
                if (!m_IsRailMoving)
                {
                    YoyoOpen();
                }
            }
            else
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        else
        {
            m_Rigidbody.constraints = RigidbodyConstraints.None;
            m_Rigidbody.velocity = Vector3.zero;
            m_IsRailMoving = false;
            m_IsCollised = false;
            //if (m_Space >= m_SpaceDisMax && !m_IsRailMoving)
            //{
            //    YoyoOpen();
            //}
            YoyoReturn();
        }

        //縦でレールを挟んだら移動
        if (m_IsCollised && m_TargetCollider.tag == "Ball" && !m_IsHorizontal)
        {
            YoyoClose();
            m_IsRailMoving = true;
            m_Rigidbody.constraints = RigidbodyConstraints.None;
            transform.LookAt(m_TargetCollider.transform.parent.FindChild("Cube").transform);

            if ((transform.position.x - m_TargetCollider.transform.parent.FindChild("Cube").transform.position.x) != 0)
            {
                Debug.Log(transform.position);
                transform.position += (transform.forward * Time.deltaTime * 2);
            }
            else
            {
                m_IsRailMoving = false;
                YoyoReturn();
            }
        }

        //横で鉄骨とかを挟んでターザン移動
        if (m_IsCollised && m_TargetCollider.tag == "Cube" && m_IsHorizontal)
        {
            YoyoClose();
            m_Rigidbody.constraints = RigidbodyConstraints.FreezePosition;

            m_Rigidbody.useGravity = true;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.Find("Left").transform.position, -transform.Find("Left").transform.up, out hit, m_SpaceDisMax))
        {
            if (hit.collider.tag == "Ball")
            {
                m_IsCollised = true;
                m_TargetCollider = hit.collider;
            }
            else if (hit.collider.tag == "Cube")
            {
                m_IsCollised = true;
                m_TargetCollider = hit.collider;
            }
            //else if(hit.collider.tag == "Rail")
            //{
            //    m_IsCollised = true;
            //    m_TargetCollider = hit.collider;
            //}
            else
            {
                m_IsCollised = false;
            }
        }

        Debug.DrawRay(transform.Find("Left").transform.position, -transform.Find("Left").transform.up * 3, Color.red);

        Debug.Log(m_Space);
    }

    private void GetPrototypeAttribute()
    {
        m_ScalePrototype = this.transform.localScale;
        //m_PosPrototype = m_HandOrigin.transform.position;
        m_PosPrototype = this.transform.position;
        m_RotPrototype = this.transform.rotation;
    }

    //ヨーヨーの開き処理
    private void YoyoOpen()
    {
        //開く
        if (m_Space < m_SpaceDisMax)
        {
            m_Left.transform.position += this.transform.right * -m_SeparationSpeed;
            m_Right.transform.position += this.transform.right * m_SeparationSpeed;
        }
        if (m_Space > m_SpaceDisMax)
        {
            m_Space = m_SpaceDisMax;
        }

        //拡大
        //this.transform.localScale += new Vector3(m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime);

    }

    //ヨーヨーの閉じ処理
    private void YoyoClose()
    {
        //閉じる
        if (m_Space > m_SpaceDisMin)
        {
            m_Left.transform.position += this.transform.right * m_SeparationSpeed;
            m_Right.transform.position += this.transform.right * -m_SeparationSpeed;
        }
        if (m_Space < m_SpaceDisMin)
        {
            m_Space = m_SpaceDisMin;
        }
    }

    private void YoyoReturn()
    {
        //元の場所に戻る
        if (transform.position.x != m_PosPrototype.x && m_IsOpened)
        {
            transform.Translate(Vector3.forward * -m_ThrowDis * Time.deltaTime * 5f);
        }

        //大きさを戻す
        if (this.transform.localScale.x > m_ScalePrototype.x)
        {
            this.transform.localScale -= new Vector3(m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime);
            if (this.transform.localScale.x < m_ScalePrototype.x)
            {
                this.transform.localScale = m_ScalePrototype;
            }
        }
        //場所を戻す
        if (transform.position.x != m_PosPrototype.x)
        {
            transform.position = m_PosPrototype;
        }
        //向きを戻す
        if (transform.rotation.x != m_RotPrototype.x)
        {
            transform.rotation = m_RotPrototype;
        }

        m_IsOpened = false;
        //m_TargetCollider = null;
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }

    public bool IsCollised()
    {
        return m_IsCollised;
    }
}
