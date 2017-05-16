using UnityEngine;
using System.Collections;

public class ShotGripper : MonoBehaviour
{
    ////球rayの半径
    //public float radius;
    ////球rayの飛ばす距離
    //public float direction;

    //private Vector3 position;
    //private float distance = -30.0f;
    RopeSimulate ropeSimulate;
    Player player;

    Vector3 movePos;
    Vector3 point;

    public GameObject Rope;
    public GameObject CopyRope = null;
    private GameObject target;

    public float speed;

    bool IsBullet = false;
    bool IsIK = false;

    void Awake()
    {
        target = GameObject.Find("Player");
        player = target.GetComponent<Player>();
        point = player.HitPoint;
    }
    void Start()
    {
        //生成した時にプレイヤーと接触して微妙に位置がずれるため
        //最初はオフ
        //GetComponent<SphereCollider>().enabled = false;
        //弾を飛ばす
        ShotBullet();
        //ropeSimulate.SimulationStop();
    }

    void Update()
    {
        ////ロープの発射
        //if (IsBullet == false)
        //{
        //    ropeSimulate.originPosition = transform.position;
        //}
        //物理挙動をしている時はプレイヤーとロープの末尾を同期
        if (IsBullet == true && IsIK == true)
        {
            player.transform.position = ropeSimulate.tailPosition;
        }
        //物理挙動をシていない時はロープの末尾とプレイヤーを同期
        else ropeSimulate.tailPosition = getPlayerPosition;

        //ロープの挙動on
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            IsIK = true;
            ropeSimulate.SimulationStart();
        }
        //ロープの巻き取り(ロープが移動)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MoveToPlayerPosition_(transform.position);
            //ropeSimulate.SimulationEnd(target.transform);
        }
        //ロープの巻き取り(プレイヤーが移動)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MoveToOriginRope_(getPlayerPosition);
            //ropeSimulate.SimulationEnd(transform);
        }
    }
    public void ShotBullet()
    {
        CopyRope = Instantiate(Rope, transform.position, Quaternion.identity);

        ropeSimulate = CopyRope.GetComponent<RopeSimulate>();
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
        float distance = Vector3.Distance(current, target);
        while (distance >= speed * Time.unscaledDeltaTime + 0.01f)
        {
            transform.position = Vector3.MoveTowards(current, target, speed * Time.deltaTime);
            ropeSimulate.originPosition = getPosition;

            current = getPosition;
            distance = Vector3.Distance(current, target);
            yield return null;
        }
        ropeSimulate.originPosition = target;

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
        float distance = Vector3.Distance(current, getPlayerPosition);

        while (distance >= speed * Time.unscaledDeltaTime + 0.01f)
        {
            transform.position = Vector3.MoveTowards(current, getPlayerPosition, speed * Time.deltaTime);

            current = getPosition;
            ropeSimulate.originPosition = current;
            distance = Vector3.Distance(current, getPlayerPosition);
            yield return null;
        }
        transform.position = getPlayerPosition;
        IsBullet = true;
        Destroy(gameObject);
        //プレイヤーと接触して位置がずれるためoffにする
        //GetComponent<SphereCollider>().enabled = false;
        //for (float i = 0.0f; i < 0.5f; i += Time.deltaTime)
        //{
        //    float t = i / 0.5f;

        //    transform.position = Vector3.Lerp(current, getPlayerPosition, t);
        //    //ropeSimulate.originPosition = transform.position;

        //    yield return null;
        //}
        //transform.position = getPlayerPosition;
        //IsBullet = true;

        //Destroy(gameObject);
    }

    //Player→OriginRope
    Coroutine MoveToOriginRope_(Vector3 current)
    {
        return StartCoroutine(MoveToOriginRope(current));
    }

    //現在地
    IEnumerator MoveToOriginRope(Vector3 current)
    {
        float distance = Vector3.Distance(current, transform.position);

        while (distance >= speed * Time.unscaledDeltaTime + 0.01f)
        {
            player.transform.position = Vector3.MoveTowards(current, transform.position, speed * Time.deltaTime);

            current = getPlayerPosition;

            distance = Vector3.Distance(current, transform.position);
            yield return null;
            Debug.Log("a");
        }
        player.transform.position = ropeSimulate.originPosition;
        IsBullet = true;
        Destroy(gameObject);

        //プレイヤーと接触して位置がずれるためoffにする
        //GetComponent<SphereCollider>().enabled = false;
        //Vector3 pf = player.transform.forward.normalized;

        //for (float i = 0.0f; i < 0.5f; i += Time.deltaTime)
        //{
        //    float t = i / 0.5f;
        //    //                                                                     プレイヤーの半径分マイナス
        //    player.transform.position = Vector3.Lerp(current, ropeSimulate.originPosition - pf * 0.5f, t);

        //    //player.transform.position = Vector3.Lerp(getPlayerPosition, ropeSimulate.originPosition, t);

        //    yield return null;
        //}

        //player.transform.position = ropeSimulate.originPosition - pf * 0.5f;
        //IsBullet = true;

        //Destroy(gameObject);
    }

    //------------------------同じような事書いてるので要修正-------------------------------

    Vector3 getPlayerPosition
    {
        get { return player.Position; }
    }
    Vector3 getPosition
    {
        get { return transform.position; }
    }
}
