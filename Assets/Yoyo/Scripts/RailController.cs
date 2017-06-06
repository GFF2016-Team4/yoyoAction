using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour
{
    [SerializeField]
    public Transform m_Player;
    [SerializeField]
    public LayerMask layerMask;
    [HideInInspector]
    private DirectionState m_State;

    Vector3 dir;
    Vector3 from, to;
    private float angle;
    private float dot1;
    private float dot2;
    private Collider[] hitColliders;

    private RaycastHit hitInfo;

    public enum DirectionState
    {
        Forward,
        Backward,
        Left,
        Right
    }

    // Use this for initialization
    void Start()
    {
        //m_State = "";
    }

    // Update is called once per frame
    void Update()
    {
        //var center = transform.position;
        //var halfExtent = transform.GetChild(0).position - transform.position;
        //hitColliders = Physics.OverlapBox(center, halfExtent, Quaternion.identity, layerMask);
        //int i = 0;
        //while (i < hitColliders.Length)
        //{
        //    Debug.Log(hitColliders[i]);
        //    i++;
        //}

        if (Physics.BoxCast(transform.GetChild(0).position, transform.GetChild(0).localScale * 0.5f, (transform.GetChild(1).position - transform.GetChild(0).position), out hitInfo, Quaternion.identity, layerMask));
        {
            Debug.Log(hitInfo.transform.name);
        }

        checkTargetDirForMe(m_Player);

        //Debug.Log(angle);
        //Debug.Log(m_State);
    }

    //角度と前後左右を求める 
    public void checkTargetDirForMe(Transform target)
    {
        Vector3 dir = target.position - transform.position; //距離差と方向 
                                                            //ドット積 
                                                            //ドット積の計算方式は: a·b =| a |·| b | cos < a,b > 其の中 | a | 和 | b | はベクトルの長さ 。  
        dot1 = Vector3.Dot(transform.forward, dir.normalized);//ドット積で前後判断   //dot >0は前  <0は後  
        if (dot1 > 0)
        {
            //Debug.Log("前");
        }
        else
        {
            //Debug.Log("後");
        }
        dot2 = Vector3.Dot(transform.right, dir.normalized);//ドット積で左右判断   //dot1>0は右  <0は左    
        if (dot2 > 0)
        {
            //Debug.Log("右");
        }
        else
        {
            //Debug.Log("左");
        }
        Vector3 from, to;
        from = transform.forward;
        to = dir;

        from.y = 0;
        to.y = 0;
        angle = Mathf.Acos(Vector3.Dot(from.normalized, to.normalized)) * Mathf.Rad2Deg;//ドット積で角度を求める

        if (angle > 150 && angle < 180)
        {
            //m_State = "back";
            m_State = DirectionState.Backward;
        }
        else if (angle > 0 && angle < 30)
        {
            //m_State = "front";
            m_State = DirectionState.Forward;
        }
        else if (angle > 60 && angle < 120)
        {
            if (dot2 > 0)
                m_State = DirectionState.Right;
            //m_State = "right";
            else
                m_State = DirectionState.Left;
                //m_State = "left";
        }
        else
        {
            //m_State = "";
        }
    }

    public DirectionState GetState()
    {
        return m_State;
    }
}
