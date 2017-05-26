using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoController : MonoBehaviour
{
    [SerializeField, Header("最小ヨーヨー間隔")]
    public float m_SpaceDisMin = 0.6f;
    [SerializeField, Header("最大ヨーヨー間隔")]
    public float m_SpaceDisMax = 3.0f;
    [SerializeField, Header("速度")]
    public float m_Speed = 1f;
    [SerializeField, Header("最大速度")]
    public float m_SpeedMax = 5f;
    [SerializeField, Header("最小速度")]
    public float m_SpeedMin = 1f;
    [SerializeField, Header("加速倍率")]
    public float m_SpeedFactor = 2f;
    [SerializeField, Header("ロープの長さ制限")]
    public float m_RopeDis = 10f;

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
    public RopeSimulate ropeSimulate;
    private Transform ropeOrigin;
    private Player m_Player;

    private Vector3 movePos;
    private Vector3 point;

    private Rope m_Rope;

    public GameObject Rope;
    private GameObject CopyRope = null;

    private bool m_IsOpened = false;            //開いたか?
    private bool m_IsCollised = false;          //当たったか?
    private bool m_IsYoyoHorizontal = false;        //水平であるか?
    private bool m_IsRailMoving = false;        //レール移動中であるか?

    bool IsBullet = false;
    bool IsIK = false;

    public float yoyoSpeed = 20;
    void Awake()
    {
        m_Left = transform.FindChild("Left").gameObject;
        m_Right = transform.FindChild("Right").gameObject;

        m_Player = GameObject.Find("Player").GetComponent<Player>();
        m_Rail = FindObjectOfType<RailController>().transform.GetComponent<RailController>();

        m_Rigidbody = transform.GetComponent<Rigidbody>();

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
        Mathf.Clamp(m_Speed, m_SpeedMin, m_SpeedMax);

        Debug.DrawRay(m_Left.transform.position, -m_Left.transform.up * 10f, Color.black);
        RaycastHit hit;
        if (Physics.SphereCast(m_Left.transform.position, 0.5f, -m_Left.transform.up, out hit, m_SpaceDisMax))
        {
            //Debug.Log(hit.collider.name);
            m_TargetCollider = hit.collider;
            m_IsCollised = true;
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
            if (!m_Player.PlayerIsGround/* && m_Player.hitInfo.transform.tag != "Pillar"*/)
            {
                IsIK = true;
            }

            //m_Player.transform.position = ropeSimulate.tailPosition;

            //m_Player.hitShotをアクセスできないと他の建物に挟んでもレール移動
            if (m_TargetCollider.tag == "Rail" && m_Rail.GetState() == "前")
            {
                m_Speed += m_Player.PlayerSpeed;
                MoveToRailgoal_(transform.position, m_TargetCollider.transform.GetChild(0).transform.position);

                ropeOrigin.GetComponent<SphereCollider>().enabled = true;
            }
            else if (m_TargetCollider.tag == "Rail" && m_Rail.GetState() == "後")
            {
                m_Speed += m_Player.PlayerSpeed;
                MoveToRailgoal_(transform.position, m_TargetCollider.transform.GetChild(1).transform.position);

                ropeOrigin.GetComponent<SphereCollider>().enabled = true;
            }
            else if (m_TargetCollider.tag == "Rail" && m_Rail.GetState() == "左")
            {

            }
            else if (m_TargetCollider.tag == "Rail" && m_Rail.GetState() == "右")
            {

            }
        }

        //ヨーヨー移動制御
        if (Input.GetKeyDown(KeyCode.F))
        {
            m_IsOpened = true;
        }
        else
        {
            m_IsRailMoving = false;
            m_IsCollised = false;

            StartCoroutine(YoyoClose());
        }

        if (m_IsOpened)
        {
            StartCoroutine(YoyoOpen());
        }

        //ロープの巻き取り(ロープが移動)
        if ((IsBullet && Input.GetMouseButtonUp(0)) || (IsBullet && Input.GetKeyDown(KeyCode.Space)))
        {
            //Vector3 direction = ropeSimulate.direction;
            //direction.y = 0;
            //m_Player.MoveDirection = direction.normalized;

            //m_Player.ResetGravity();
            //m_Player.PlayerSpeed = ropeSimulate.GetRopeSpeed();

            IsBullet = false;

            MoveToPlayerPosition_(transform.position);

            //m_Player.PlayerSpeed = m_Speed;
            m_Player.SideMove();

            ropeSimulate.SimulationEnd(transform);
        }
        //ロープの巻き取り(プレイヤーが移動)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            IsBullet = false;

            MoveToOriginRope_(getPlayerPosition);
            ropeSimulate.SimulationEnd(transform);
        }

        //間隔を計算  二回の強制キャストで誤差を無くす
        m_Space = (float)((int)Vector3.Distance(m_Left.transform.position, m_Right.transform.position));

        //ヨーヨーの回転制御
        if (m_Rail.GetState() == "前" || m_Rail.GetState() == "後")
        {
            m_IsYoyoHorizontal = false;
        }
        else if (m_Rail.GetState() == "左" || m_Rail.GetState() == "右")
        {
            m_IsYoyoHorizontal = true;
        }

        //Debug.Log(m_Rail.GetState());
        //Debug.Log(m_IsCollised);
        //Debug.Log("sphereCast" + Physics.Linecast(transform.Find("Left").transform.position, transform.Find("Right").transform.position, out hit));
        Debug.DrawLine(m_Left.transform.position, -m_Left.transform.right * m_SpaceDisMax, Color.cyan);
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

        m_IsOpened = true;

        MoveToTarget_(transform.position, point);
    }

    //OriginRope→target
    Coroutine MoveToTarget_(Vector3 current, Vector3 target)
    {
        return StartCoroutine(MoveToTarget(current, target));
    }

    //現在地、目的地
    IEnumerator MoveToTarget(Vector3 current, Vector3 target)
    {
        float distance = Vector3.Distance(current, target);

        while (distance >= yoyoSpeed * Time.unscaledDeltaTime + 0.01f)
        {
            transform.position = Vector3.MoveTowards(current, target, yoyoSpeed * Time.deltaTime);

            current = transform.position;
            ropeSimulate.originPosition = current;
            distance = Vector3.Distance(current, target);
            yield return null;
        }
        transform.position = target;

        //移動が終わったらコライダーを戻す
        //GetComponent<SphereCollider>().enabled = true;

        IsBullet = true;

        //空中 + 柱以外に当たった時物理挙動on-----
        if (/*m_Player.hitInfo.transform.tag != "Pillar" &&*/ !m_Player.PlayerIsGround)
        {
            ropeSimulate.SimulationStart();
        }
        //-----
        Vector3 ropeDirection = ropeSimulate.direction + m_Player.MoveDirection;
        ropeDirection.y = 0;

        ropeSimulate.AddForce(ropeDirection + Vector3.down * m_Player.PlayerSpeed, ForceMode.Impulse);
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

        MoveToPlayerPosition_(transform.position);
        ropeSimulate.SimulationEnd(transform);

        m_IsOpened = false;

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
        float distance = Vector3.Distance(current, transform.position);

        while (distance >= yoyoSpeed * Time.unscaledDeltaTime + 0.01f)
        {
            m_Player.transform.position = Vector3.MoveTowards(current, transform.position, yoyoSpeed * Time.deltaTime);

            current = getPlayerPosition;

            distance = Vector3.Distance(current, transform.position);
            yield return null;
            //Debug.Log("a");
        }
        m_Player.transform.position = ropeSimulate.originPosition;

        IsBullet = true;

        Destroy(gameObject);
    }

    //レールの端まで移動
    Coroutine MoveToRailgoal_(Vector3 current, Vector3 target)
    {
        return StartCoroutine(MoveToRailgoal(current, target));
    }

    //現在地、目的地
    IEnumerator MoveToRailgoal(Vector3 current, Vector3 target)
    {
        //Vector3 ropeDis = ropeSimulate.originPosition - ropeSimulate.tailPosition;
        //Vector3 currentOrigin;
        //Vector3 previosOrigin;

        float distance = Vector3.Distance(current, target);

        while (distance >= m_Speed * Time.unscaledDeltaTime + 0.01f)
        {
            //previosOrigin = ropeSimulate.originPosition;
            transform.position = Vector3.MoveTowards(current, target, m_Speed * Time.deltaTime);

            current = transform.position;
            ropeSimulate.originPosition = current;

            //currentOrigin = ropeSimulate.originPosition;
            //ropeSimulate.tailPosition = previosOrigin - currentOrigin;

            //current += ropeDis;
            //ropeSimulate.tailPosition = current;

            distance = Vector3.Distance(current, target);
            yield return null;
        }
        transform.position = target;

        //m_Player.AccelAdd();
        m_Player.PlayerSpeed = m_Speed;

        IsBullet = false;

        MoveToPlayerPosition_(transform.position);
        ropeSimulate.SimulationEnd(transform);

        m_IsOpened = false;
    }

    //元の位置情報などを一旦取得して保存
    private void GetPrototypeAttribute()
    {
        if (m_Player)
            m_PosPrototype = m_Player.transform.position;
        else
            m_PosPrototype = this.transform.position;

        m_ScalePrototype = this.transform.localScale;
        m_RotPrototype = this.transform.rotation;
    }

    //ヨーヨーの開き処理
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


    Vector3 getPlayerPosition
    {
        get { return m_Player.Position; }
    }

    public bool IsCollised()
    {
        return m_IsCollised;
    }
    public bool NowBullet()
    {
        return IsBullet;
    }
}
