using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoController : MonoBehaviour
{
    [SerializeField, Header("プレイヤー")]
    public Transform m_Player;

    [SerializeField, Header("最小ヨーヨー間隔")]
    public float m_SpaceDisMin = 0.6f;
    [SerializeField, Header("最大ヨーヨー間隔")]
    public float m_SpaceDisMax = 3.0f;
    [SerializeField, Header("投げ出す距離")]
    public float m_ThrowDis;
    [SerializeField, Header("速度")]
    public float m_Speed =10f;
    [SerializeField, Header("加速倍率")]
    public float m_SpeedFactor = 2f;

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
    private RailController m_Rail;

    private bool m_IsOpened = false;            //開いたか?
    private bool m_IsCollised = false;          //当たったか?
    private bool m_IsHorizontal = false;        //水平であるか?
    private bool m_IsRailMoving = false;        //レール移動中であるか?

    // Use this for initialization
    void Start()
    {
        m_Left = transform.FindChild("Left").gameObject;
        m_Right = transform.FindChild("Right").gameObject;
        
        m_Player = transform.Find("Player");

        m_Rigidbody = transform.GetComponent<Rigidbody>();
        m_Rail = FindObjectOfType<RailController>().GetComponent<RailController>();
        //手元の大きさと場所を取得
        GetPrototypeAttribute();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(m_Rail.GetState());
        //間隔を計算
        m_Space = (float)((int)Vector3.Distance(m_Left.transform.position, m_Right.transform.position));

        //ヨーヨーの回転制御
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_IsHorizontal = !m_IsHorizontal;
        }

        //ヨーヨー移動制御
        if(Input.GetKey(KeyCode.F))
        {
            StartCoroutine(YoyoOpen());
        }
        else
        {
            m_IsRailMoving = false;
            m_IsCollised = false;

            StartCoroutine(YoyoClose());
        }

        if (m_IsCollised && m_TargetCollider.tag == "Rail")
        {
            StartCoroutine(YoyoClose());
            m_Rigidbody.constraints = RigidbodyConstraints.None;

            //縦でレールを挟んだら移動
            if (m_Rail.GetState() == "後" && !m_IsRailMoving && m_IsHorizontal)
            {
                transform.SetPositionAndRotation(this.transform.position, m_TargetCollider.transform.parent.transform.rotation);
                m_IsRailMoving = true;
            }
            else if(m_Rail.GetState() == "前" && !m_IsRailMoving && m_IsHorizontal)
            {
                transform.SetPositionAndRotation(this.transform.position, Quaternion.Inverse(m_TargetCollider.transform.parent.transform.rotation));
                m_IsRailMoving = true;
            }

            //横でレールを挟んでワイヤーアクション
            else if (m_Rail.GetState() == "左" && !m_IsRailMoving && !m_IsHorizontal)
            {
                m_IsRailMoving = true;
            }
            else if (m_Rail.GetState() == "右" && !m_IsRailMoving && !m_IsHorizontal)
            {
                m_IsRailMoving = true;
            }

            //斜めで二分の一の速度で移動しつつワイヤーアクション
            //else if(m_Rail.GetState() == "")

            //加速
            m_Rigidbody.AddForce(transform.forward * m_SpeedFactor * m_Speed, ForceMode.VelocityChange);
        }
        RaycastHit hit;
        Debug.Log("sphereCast" + Physics.SphereCast(transform.Find("Left").transform.position, 0.5f, -transform.Find("Left").transform.up, out hit, m_SpaceDisMax));

        //RaycastHit hit;
        if (Physics.SphereCast(transform.Find("Left").transform.position, 0.5f, -transform.Find("Left").transform.up, out hit, m_SpaceDisMax))
        {
            //レールとの判定
            if (hit.collider.tag == "Rail")
            {
                m_IsCollised = true;
                m_TargetCollider = hit.collider;
            }
            else
            {
                m_IsCollised = false;
            }
        }

        Debug.DrawLine(transform.Find("Left").transform.position, -transform.Find("Left").transform.forward * m_SpaceDisMax, Color.cyan);

        //Debug.Log("Local: " + transform.position);
    }

    private void GetPrototypeAttribute()
    {
        if (m_Player)
            m_PosPrototype = m_Player.transform.position;
        else
            m_PosPrototype = this.transform.position;

        m_ScalePrototype = this.transform.localScale;
        m_RotPrototype = this.transform.rotation;
    }

    ////ヨーヨーの開き処理
    //private void YoyoOpen()
    //{
    //    //開く
    //    if (m_Space < m_SpaceDisMax)
    //    {
    //        m_Left.transform.position += this.transform.right * -m_SeparationSpeed;
    //        m_Right.transform.position += this.transform.right * m_SeparationSpeed;
    //    }
    //    if (m_Space > m_SpaceDisMax)
    //    {
    //        m_Space = m_SpaceDisMax;
    //    }

    //    //拡大
    //    //this.transform.localScale += new Vector3(m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime);
    //}

    ////ヨーヨーの閉じ処理
    //private void YoyoClose()
    //{
    //    //閉じる
    //    if (m_Space > m_SpaceDisMin)
    //    {
    //        m_Left.transform.position += this.transform.right * m_SeparationSpeed;
    //        m_Right.transform.position += this.transform.right * -m_SeparationSpeed;
    //    }
    //    if (m_Space < m_SpaceDisMin)
    //    {
    //        m_Space = m_SpaceDisMin;
    //    }
    //}

    //private void YoyoReturn()
    //{
    //    //元の場所に戻る
    //    //if (transform.position.x != m_PosPrototype.x && m_IsOpened)
    //    //{
    //    //    transform.Translate(Vector3.forward * -m_ThrowDis * Time.deltaTime * 5f);
    //    //}

    //    //大きさを戻す
    //    if (this.transform.localScale.x > m_ScalePrototype.x)
    //    {
    //        this.transform.localScale -= new Vector3(m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime);
    //        if (this.transform.localScale.x < m_ScalePrototype.x)
    //        {
    //            this.transform.localScale = m_ScalePrototype;
    //        }
    //    }
    //    //場所を戻す
    //    if (transform.position.x != m_PosPrototype.x)
    //    {
    //        transform.position = m_PosPrototype;
    //    }
    //    //向きを戻す
    //    if (transform.rotation.x != m_RotPrototype.x)
    //    {
    //        transform.rotation = m_RotPrototype;
    //    }

    //    m_IsOpened = false;
    //    //m_TargetCollider = null;
    //}

    IEnumerator YoyoOpen()
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

        yield return null;
    }

    //ヨーヨーの閉じ処理
    IEnumerator YoyoClose()
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

        yield return null;
    }

    IEnumerator YoyoReturn()
    {
        //大きさを戻す
        if (this.transform.localScale.x > m_ScalePrototype.x)
        {
            this.transform.localScale -= new Vector3(m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime);
            if (this.transform.localScale.x < m_ScalePrototype.x)
            {
                this.transform.localScale = m_ScalePrototype;
            }
        }
        //向きを戻す
        if (transform.rotation.x != m_RotPrototype.x)
        {
            transform.rotation = m_RotPrototype;
        }

        m_IsOpened = false;

        yield return null;
    }

    public bool IsCollised()
    {
        return m_IsCollised; 
    }
}
