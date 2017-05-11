using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoController : MonoBehaviour
{
    [SerializeField, Header("最小ヨーヨー間隔")]
    public float m_SpaceDisMin = 0.6f;
    [SerializeField, Header("最大ヨーヨー間隔")]
    public float m_SpaceDisMax = 3.0f;
    [SerializeField, Header("投げ出す距離")]
    public float m_ThrowDis;
    [SerializeField, Header("速度")]
    public float m_Speed = 10f;
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
    private ShotGripper m_ShotGripper;
    private RopeSimulate ropeSimulate;
    private Transform ropeOrigin;
    private Player m_Player;

    private Vector3 movePos;
    private Vector3 point;

    public GameObject Rope;
    public GameObject CopyRope = null;
    private GameObject target;

    private bool m_IsOpened = false;            //開いたか?
    private bool m_IsCollised = false;          //当たったか?
    private bool m_IsHorizontal = false;        //水平であるか?
    private bool m_IsRailMoving = false;        //レール移動中であるか?

    bool IsBullet = false;
    bool IsIK = false;

    void Awake()
    {
        m_Left = transform.FindChild("Left").gameObject;
        m_Right = transform.FindChild("Right").gameObject;

        target = GameObject.Find("Player");
        m_Player = target.GetComponent<Player>();

        m_ShotGripper = transform.GetComponent<ShotGripper>();

        m_Rigidbody = transform.GetComponent<Rigidbody>();
        m_Rail = FindObjectOfType<RailController>().transform.GetComponent<RailController>();
        //手元の大きさと場所を取得
        GetPrototypeAttribute();

        point = m_Player.HitPoint;
    }

    // Use this for initialization
    void Start()
    {
        //弾を飛ばす
        ShotBullet();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.Find("Left").transform.position, -transform.Find("Left").up * 10f, Color.black);
        RaycastHit hit;
        if (Physics.SphereCast(transform.Find("Left").transform.position, 0.5f, -transform.Find("Left").up, out hit))
        {
            Debug.Log(hit.collider.name);
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

        //ロープの発射
        if (IsBullet == false)
        {
            ropeSimulate.originPosition = transform.position;
        }
        //物理挙動をしている時はプレイヤーとロープの末尾を同期
        if (IsBullet == true && IsIK == true)
        {
            m_Player.transform.position = ropeSimulate.tailPosition;
        }
        //物理挙動をしていない時はロープの末尾とプレイヤーを同期
        else ropeSimulate.tailPosition = getPlayerPosition;

        //ロープの挙動on
        if (IsBullet == true)
        {
            IsIK = true;
            ropeSimulate.SimulationStart();

            if (m_TargetCollider.tag == "Rail" && m_Rail.GetState() == "前")
            {
                //transform.rotation = new Quaternion(-m_TargetCollider.transform.forward.x, -m_TargetCollider.transform.forward.y, -m_TargetCollider.transform.forward.z, 0f);
                MoveToRailgoal_(transform.position, m_TargetCollider.transform.parent.GetChild(3).transform.GetChild(0).transform.position);

                ropeOrigin.GetComponent<SphereCollider>().enabled = true;
            }
            else if(m_TargetCollider.tag == "Rail" && m_Rail.GetState() == "後")
            {
                //transform.rotation = new Quaternion(m_TargetCollider.transform.forward.x, m_TargetCollider.transform.forward.y, m_TargetCollider.transform.forward.z, 0f);
                MoveToRailgoal_(transform.position, m_TargetCollider.transform.parent.GetChild(3).transform.GetChild(1).transform.position);

                ropeOrigin.GetComponent<SphereCollider>().enabled = true;
            }

            //MoveToTarget_(transform.position, m_TargetCollider.transform.forward);
        }
        //ロープの巻き取り(ロープが移動)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            IsBullet = false;

            MoveToPlayerPosition_(transform.position);
            ropeSimulate.SimulationEnd(target.transform);
        }
        //ロープの巻き取り(プレイヤーが移動)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            IsBullet = false;

            MoveToOriginRope_(getPlayerPosition);
            ropeSimulate.SimulationEnd(transform);
        }

        //間隔を計算
        m_Space = (float)((int)Vector3.Distance(m_Left.transform.position, m_Right.transform.position));

        //ヨーヨーの回転制御
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_IsHorizontal = !m_IsHorizontal;
        }

        //ヨーヨー移動制御
        if (Input.GetKey(KeyCode.F))
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
            //m_Rigidbody.constraints = RigidbodyConstraints.None;

            ////縦でレールを挟んだら移動
            //if (m_Rail.GetState() == "後" && !m_IsRailMoving && m_IsHorizontal)
            //{
            //    transform.SetPositionAndRotation(this.transform.position, m_TargetCollider.transform.parent.transform.rotation);
            //    m_IsRailMoving = true;
            //}
            //else if(m_Rail.GetState() == "前" && !m_IsRailMoving && m_IsHorizontal)
            //{
            //    transform.SetPositionAndRotation(this.transform.position, Quaternion.Inverse(m_TargetCollider.transform.parent.transform.rotation));
            //    m_IsRailMoving = true;
            //}
            
            ////横でレールを挟んでワイヤーアクション
            //else if (m_Rail.GetState() == "左" && !m_IsRailMoving && !m_IsHorizontal)
            //{
            //    m_IsRailMoving = true;
            //}
            //else if (m_Rail.GetState() == "右" && !m_IsRailMoving && !m_IsHorizontal)
            //{
            //    m_IsRailMoving = true;
            //}

            //加速
            //m_Rigidbody.AddForce(transform.forward * m_SpeedFactor * m_Speed, ForceMode.VelocityChange);

            //transform.Translate(transform.forward);
        }

        //Debug.Log(m_Rail.GetState());
        Debug.Log(m_IsCollised);
        //Debug.Log("sphereCast" + Physics.Linecast(transform.Find("Left").transform.position, transform.Find("Right").transform.position, out hit));
        Debug.DrawLine(transform.Find("Left").transform.position, -transform.Find("Left").transform.right * m_SpaceDisMax, Color.cyan);
    }

    public void ShotBullet()
    {
        CopyRope = Instantiate(Rope, transform.position, Quaternion.identity);

        ropeSimulate = CopyRope.GetComponent<RopeSimulate>();
        ropeOrigin = CopyRope.transform.FindChild("Origin").transform.GetComponent<Transform>();

        ropeOrigin.GetComponent<SphereCollider>().enabled = false;

        CopyRope.transform.parent = transform;

        //初期化           引数(origin,tail) 
        ropeSimulate.InitPosition(transform.position, getPlayerPosition);
        //最初は物理挙動off
        ropeSimulate.SimulationStop();

        MoveToTarget_(transform.position, point);
    }

    //------------------------同じような事書いてるので要修正-------------------------------
    //OriginRope→target
    Coroutine MoveToTarget_(Vector3 current, Vector3 target)
    {
        return StartCoroutine(MoveToTarget(current, target));
    }

    //現在地、目的地
    IEnumerator MoveToTarget(Vector3 current, Vector3 target)
    {
        for (float i = 0.0f; i < 1f; i += Time.deltaTime)
        {
            float t = i / 1f;

            movePos = target;
            transform.position = Vector3.Lerp(current, movePos, t);
            ropeSimulate.originPosition = transform.position;

            yield return null;
        }
        transform.position = movePos;

        //移動が終わったらコライダーを戻す
        //GetComponent<SphereCollider>().enabled = true;

        IsBullet = true;
    }

    //OriginRope→Player
    Coroutine MoveToPlayerPosition_(Vector3 current)
    {
        return StartCoroutine(MoveToPlayerPosition(current));
    }

    //現在地
    IEnumerator MoveToPlayerPosition(Vector3 current)
    {
        //プレイヤーと接触して位置がずれるためoffにする
        //GetComponent<SphereCollider>().enabled = false;
        for (float i = 0.0f; i < 0.5f; i += Time.deltaTime)
        {
            float t = i / 0.5f;

            transform.position = Vector3.Lerp(current, getPlayerPosition, t);
            //ropeSimulate.originPosition = transform.position;

            yield return null;
        }
        transform.position = getPlayerPosition;
        IsBullet = true;

        Destroy(gameObject);
    }

    //Player→OriginRope
    Coroutine MoveToOriginRope_(Vector3 current)
    {
        return StartCoroutine(MoveToOriginRope(current));
    }

    //現在地
    IEnumerator MoveToOriginRope(Vector3 current)
    {
        //プレイヤーと接触して位置がずれるためoffにする
        //GetComponent<SphereCollider>().enabled = false;
        Vector3 pf = m_Player.transform.forward.normalized;

        for (float i = 0.0f; i < 0.5f; i += Time.deltaTime)
        {
            float t = i / 0.5f;
            //                                                                     プレイヤーの半径分マイナス
            m_Player.transform.position = Vector3.Lerp(current, ropeSimulate.originPosition - pf * 0.5f, t);

            //player.transform.position = Vector3.Lerp(getPlayerPosition, ropeSimulate.originPosition, t);

            yield return null;
        }

        m_Player.transform.position = ropeSimulate.originPosition - pf * 0.5f;
        IsBullet = true;

        Destroy(gameObject);
    }

    //------------------------同じような事書いてるので要修正-------------------------------

    Vector3 getPlayerPosition
    {
        get { return m_Player.Position; }
    }

    //OriginRope→target
    Coroutine MoveToRailgoal_(Vector3 current, Vector3 target)
    {
        return StartCoroutine(MoveToRailgoal(current, target));
    }

    //現在地、目的地
    IEnumerator MoveToRailgoal(Vector3 current, Vector3 target)
    {
        for (float i = 0.0f; i < 1f; i += Time.deltaTime)
        {
            float t = i / 1f;

            movePos = target;
            transform.position = Vector3.Lerp(current, movePos, t);
            ropeSimulate.originPosition = transform.position;

            yield return null;
        }
        transform.position = movePos;

        //移動が終わったらコライダーを戻す
        //GetComponent<SphereCollider>().enabled = true;

        IsBullet = false;
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
