using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoController : MonoBehaviour
{
    [SerializeField, Header("速度")]
    public float m_Speed = 20f;
    [SerializeField, Header("最大速度")]
    public float m_SpeedMax = 20f;
    [SerializeField, Header("最小速度")]
    public float m_SpeedMin = 1f;

    private GameObject m_Left;                  //ヨーヨー左部分
    private GameObject m_Right;                 //ヨーヨー右部分
    private Collider m_TargetCollider;          //当たるターゲット

    private Transform ropeOrigin;               
    private Animator m_Animator;                //アニメーター
    private Player m_Player;                    //プレイヤー

    public RopeSimulate ropeSimulate;           
    public GameObject Rope;                     
    private GameObject CopyRope = null;         
    private Vector3 point;

    bool IsBullet = false;
    bool IsIK = false;

    void Awake()
    {
        //m_Left = transform.GetChild(0).FindChild("polySurface8").Find("Left").gameObject;
        //m_Right = transform.GetChild(0).FindChild("polySurface2").Find("Right").gameObject;

        m_Player = GameObject.Find("Player").GetComponent<Player>();
        m_Animator = transform.GetComponent<Animator>();

        point = m_Player.HitPoint;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Mathf.Clamp(m_Speed, m_SpeedMin, m_SpeedMax);

        RaycastHit hit;
        if (Physics.SphereCast(m_Left.transform.position, 0.5f, -m_Left.transform.up, out hit, 6))
        {
            //Debug.Log(hit.collider.name);
            m_TargetCollider = hit.collider;
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
        else ropeSimulate.tailPosition = m_Player.transform.FindChild("RightHand").position;

        //ロープの挙動on
        if (IsBullet == true)
        {
            if (!m_Player.PlayerIsGround/* && m_Player.hitInfo.transform.tag != "Pillar"*/)
            {
                IsIK = true;
            }

            if(m_Player.hitInfo.collider.tag == "Rail")
            {
                m_Animator.SetBool("IsHit", true);
                if(m_TargetCollider.tag == "Rail")
                {
                    if (m_Player.hitInfo.collider.GetComponent<RailController>().GetState() == RailController.DirectionState.Forward)
                    {
                        m_Speed += m_Player.PlayerSpeed;
                        MoveToRailgoal_(transform.position, m_TargetCollider.transform.GetChild(0).transform.position);

                        ropeOrigin.GetComponent<SphereCollider>().enabled = true;
                        Debug.Log("レール移動");
                    }
                    else if (m_Player.hitInfo.collider.GetComponent<RailController>().GetState() == RailController.DirectionState.Backward)
                    {
                        m_Speed += m_Player.PlayerSpeed;
                        MoveToRailgoal_(transform.position, m_TargetCollider.transform.GetChild(1).transform.position);

                        ropeOrigin.GetComponent<SphereCollider>().enabled = true;
                        Debug.Log("レール移動");
                    }
                    else if (m_Player.hitInfo.collider.GetComponent<RailController>().GetState() == RailController.DirectionState.Left)
                    {
                        Debug.Log("ワイヤーアクション");
                        //transform.Rotate();
                    }
                    else if (m_Player.hitInfo.collider.GetComponent<RailController>().GetState() == RailController.DirectionState.Right)
                    {
                        Debug.Log("ワイヤーアクション");
                        //transform.Rotate();
                    }
                }            
            }          
        }

        //ロープの巻き取り(ロープが移動)
        if ((IsBullet && Input.GetMouseButtonUp(0)) || (IsBullet && Input.GetKeyDown(KeyCode.Space)))
        {
            IsBullet = false;

            MoveToPlayerPosition_(transform.position);
            m_Player.SideMove();

            ropeSimulate.SimulationEnd(transform);
        }
        //ロープの巻き取り(プレイヤーが移動)
        if (IsBullet && Input.GetMouseButtonDown(1))
        {
            IsBullet = false;

            MoveToOriginRope_(m_Player.Position);
            ropeSimulate.SimulationEnd(transform);
        }

        Debug.DrawLine(m_Left.transform.position, -m_Left.transform.right * 6, Color.cyan);
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

        while (distance >= m_Speed * Time.unscaledDeltaTime + 0.01f)        //速度X時間＝距離
        {
            transform.position = Vector3.MoveTowards(current, target, m_Speed * Time.deltaTime);

            current = transform.position;
            ropeSimulate.originPosition = current;
            distance = Vector3.Distance(current, target);
            yield return null;
        }
        transform.position = target;

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

    //public void MoveToOtherRail(Transform other)
    //{
    //    this.transform.position = other.transform.position;
    //}

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

            transform.position = Vector3.Lerp(current, m_Player.Position, t);
            //ropeSimulate.originPosition = transform.position;

            yield return null;
        }
        transform.position = m_Player.Position;
        IsBullet = true;

        MoveToPlayerPosition_(transform.position);
        ropeSimulate.SimulationEnd(transform);

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

        while (distance >= m_Speed * Time.unscaledDeltaTime + 0.01f)
        {
            m_Player.transform.position = Vector3.MoveTowards(current, transform.position, m_Speed * Time.deltaTime);

            current = m_Player.Position;

            distance = Vector3.Distance(current, transform.position);
            yield return null;
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
        float distance = Vector3.Distance(current, target);

        while (distance >= m_Speed * Time.unscaledDeltaTime + 0.01f)
        {
            transform.position = Vector3.MoveTowards(current, target, m_Speed * Time.deltaTime);

            current = transform.position;
            ropeSimulate.originPosition = current;

            distance = Vector3.Distance(current, target);
            yield return null;
        }
        transform.position = target;

        m_Player.PlayerSpeed = m_Speed;

        IsBullet = false;

        MoveToPlayerPosition_(transform.position);
        ropeSimulate.SimulationEnd(transform);

        m_Player.hitInfo.collider.GetComponent<RailController>().enabled = false;
    }

    public bool NowBullet()
    {
        return IsBullet;
    }

}
