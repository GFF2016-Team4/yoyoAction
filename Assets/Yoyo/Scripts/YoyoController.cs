using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoController : MonoBehaviour
{
    [SerializeField, Header("分離速度")]
    public float m_SeparationSpeed = 1.0f;

    [SerializeField, Header("最小ヨーヨー間隔")]
    public float m_SpaceDisMin = 0.377f;

    [SerializeField, Header("最大ヨーヨー間隔")]
    public float m_SpaceDisMax = 3.0f;

    [SerializeField, Header("投げ出す距離")]
    public float m_ThrowDis;

    private float m_Distance;           //ヨーヨーの間隔
    private Vector3 m_ScalePrototype;   //ヨーヨーの大きさ(原型)
    private Vector3 m_PosPrototype;     //ヨーヨーの場所(原型)

    public float m_ScaleNum = 10.0f;           //拡大倍率

    private GameObject m_Left;          //ヨーヨー左部分
    private GameObject m_Right;         //ヨーヨー右部分

    private Collider m_TargetCollider;

    private bool m_IsOpened = false;    //開いたか?
    private bool m_IsCollised = false;  //当たったか?

    // Use this for initialization
    void Start()
    {
        m_Left = transform.GetChild(0).gameObject;
        m_Right = transform.GetChild(1).gameObject;

        //元の大きさと場所を取得
        m_ScalePrototype = this.transform.localScale;
        m_PosPrototype = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //間隔を計算
        m_Distance = Vector3.Distance(m_Left.transform.position, m_Right.transform.position);

        //ヨーヨーの開き閉じ処理
        YoyoOpenAndClose();

        //ヨーヨーの回転制御
        if (Input.GetAxis("Horizontal") < 0)
        {
            this.transform.Rotate(Vector3.forward, 45 * Time.deltaTime);
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            this.transform.Rotate(Vector3.forward, -45 * Time.deltaTime);
        }

        //レールの目的点へ
        if (m_IsCollised)
        {
            transform.LookAt(m_TargetCollider.transform.parent.GetChild(2).transform);
            transform.Translate(transform.forward * 1 * Time.deltaTime);
        }
    }

    //ヨーヨーの開き処理
    private void YoyoOpen()
    {
        m_Left.transform.position += this.transform.right * -0.1f;
        m_Right.transform.position += this.transform.right * 0.1f;

        this.transform.localScale += new Vector3(m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime);
    }

    //ヨーヨーの閉じ処理
    private void YoyoClose()
    {
        m_Left.transform.position += this.transform.right * 0.1f;
        m_Right.transform.position += this.transform.right * -0.1f;
    }

    //ヨーヨーの戻り処理（コルーチン）
    private IEnumerator YoyoReturnCorou()
    {
        if (this.transform.localScale.x > m_ScalePrototype.x)
        {
            this.transform.localScale -= new Vector3(m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime, m_ScaleNum * Time.deltaTime);
            if (this.transform.localScale.x < m_ScalePrototype.x)
            {
                this.transform.localScale = m_ScalePrototype;
            }
        }

        transform.Translate(Vector3.forward * -m_ThrowDis * Time.deltaTime);
        if (transform.position.x < m_PosPrototype.x)
        {
            transform.position = m_PosPrototype;
        }

        m_IsOpened = false;
        yield return new WaitForSeconds(0);
    }

    //ヨーヨーの開き閉じ処理(あたり判定バージョン)
    void YoyoOpenAndClose()
    {
        if (Input.GetMouseButton(0))
        {
            //ヨーヨーの開き
            if (m_Distance < m_SpaceDisMax && !m_IsOpened)
            {
                YoyoOpen();
            }
            else
            {
                m_IsOpened = true;
            }

            //ヨーヨーの閉じ
            if (m_Distance > m_SpaceDisMin && m_IsOpened && m_IsCollised)
            {
                YoyoClose();
                m_Distance = m_SpaceDisMin;
            }
            else if (m_Distance > m_SpaceDisMin && m_IsOpened && (transform.position.x - m_PosPrototype.x >= m_ThrowDis))
            {
                YoyoClose();
                m_Distance = m_SpaceDisMin;
            }

            //ヨーヨーの伸び
            if ((transform.position.x - m_PosPrototype.x < m_ThrowDis) && !m_IsCollised)
            {
                transform.Translate(Vector3.forward * m_ThrowDis * Time.deltaTime);
            }
        }

        else
        {

            if (transform.position.x != m_PosPrototype.x)
            {
                StartCoroutine(YoyoReturnCorou());
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Ball")
        {
            Debug.Log("Ball");
            m_IsCollised = true;
            m_TargetCollider = other;
        }

        if (other.transform.tag == "Cube")
        {
            Debug.Log("Cube");
            m_IsCollised = true;
            m_TargetCollider = other;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Ball")
        {
            m_IsCollised = false;
        }
        if (other.transform.tag == "Cube")
        {
            m_IsCollised = false;
        }
    }
}
