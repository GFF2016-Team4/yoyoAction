using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RopeState
{
    Shoot, 	    	//ロープが射出中状態,
    NoSimulate, 	//ロープが動かない状態,
    TarzanMove, 	//ロープが動く状態(ターザン移動),
    RailMove,   	//ロープが動く状態(レール移動),
    CircleMove, 	//ロープが動く状態(円運動),
    TakeUp 	    	//ロープが巻き取り状態,
}

public class Gripper : MonoBehaviour {

    public RopeSimulate ropeSimulate;
    public GameObject rope;
    private GameObject copyRope = null;
    private Transform ropeOrigin;

    private Animator m_Animator;
    private Player m_Player;
    private Collider m_TargetCollider;      //当たる相手のコライダー
    private RopeState m_RopeState = RopeState.Shoot;

    //デリゲートでコルバック関数を宣言
    public delegate void Callback(Collider collider);
    public Callback callback;

    // Use this for initialization
    void Start () {
        //弾を飛ばす
        ShotBullet();
    }
	
	// Update is called once per frame
	void Update () {
        switch(m_RopeState)
        {
            case RopeState.Shoot:            
                break;
            case RopeState.NoSimulate:
                break;
            case RopeState.TarzanMove:
                
                break;
            case RopeState.RailMove:
                break;
            case RopeState.CircleMove:
                break;
            case RopeState.TakeUp:
                break;
        }

	}

    public void ShotBullet()
    {
        copyRope = Instantiate(rope, transform.position, Quaternion.identity);

        ropeSimulate = copyRope.GetComponent<RopeSimulate>();
        ropeOrigin = copyRope.transform.FindChild("Origin").transform.GetComponent<Transform>();

        ropeOrigin.GetComponent<SphereCollider>().enabled = false;

        copyRope.transform.parent = transform;

        m_Animator.SetBool("IsShoot", true);

        //初期化           引数(origin,tail) 
        ropeSimulate.InitPosition(transform.position, m_Player.transform.FindChild("RightHand").position);
        //最初は物理挙動off
        ropeSimulate.SimulationStop();

        //MoveToTarget_(transform.position, point);
    }


    private void OnTriggerEnter(Collider other)
    {
        //もしレールに当たらなかったら
        if (other.tag != "Rail") return;

        //もしレールにあたったら
        if(other.tag == "Rail")
        {
            //コルバック関数を呼ぶ
            callback(other);

            m_TargetCollider = other;
        }
        
    }

    public void SetColliderCallback(Callback func)
    {
        callback = func;
    }

    public Vector3 getPlayerPosition(Vector3 playerPos)
    {
        
    }
}
