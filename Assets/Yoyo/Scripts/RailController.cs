using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour {

    [SerializeField]
    public Transform m_Player;
    [HideInInspector]
    public bool m_IsAccel = false;
    [HideInInspector]
    public bool m_IsFront;
    [HideInInspector]
    public bool m_IsOblique;

    private string m_State;

    // Use this for initialization
    void Start () {
        m_State = "";
    }
	
	// Update is called once per frame
	void Update () {
        //グリッパーとオブジェクトの角度
        Vector3 relative = transform.InverseTransformPoint(m_Player.position);
        float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

        if(angle > 60 && angle <120)    //前
        {
            m_IsAccel = true;
            m_IsFront = true;
            m_IsOblique = false;
            m_State = "前";
        }
        else if(angle < -60 && angle > -120)    //後
        {
            m_IsAccel = true;
            m_IsFront = false;
            m_IsOblique = false;
            m_State = "後";
        }
        else if(angle > -30 && angle < 30)      //左
        {
            m_IsAccel = false;
            m_IsFront = true;
            m_IsOblique = false;
            m_State = "左";
        }
        else if((angle > 150 && angle < 180) || (angle < -150 && angle > -180))     //右
        {
            m_IsAccel = false;
            m_IsFront = false;
            m_IsOblique = false;
            m_State = "右";
        }
        else if(angle > 30 && angle < 60)   //左前
        {
            m_IsAccel = true;
            m_IsFront = true;
            m_IsOblique = true;
            m_State = "左前";
        }
        else if(angle > 120 && angle < 150) //右前
        {
            m_IsAccel = false;
            m_IsFront = false;
            m_IsOblique = true;
            m_State = "右前";
        }
        else if(angle > -60 && angle < -30) //左後
        {
            m_IsAccel = true;
            m_IsFront = false;
            m_IsOblique = true;
            m_State = "左後";
        }
        else if(angle > -150 && angle < -120)   //右後
        {
            m_IsAccel = false;
            m_IsFront = false;
            m_IsOblique = true;
            m_State = "右後";
        }

        //Debug.Log("Oblique" + m_IsOblique);
        //Debug.Log("Accel" + m_IsAccel);
        //Debug.Log("Front" + m_IsFront);

        //Debug.Log("位置状態" + m_State);
        //Debug.Log(angle);
    }

    public bool IsAccel()
    {
        return m_IsAccel;
    }

    public bool IsFront()
    {
        return m_IsFront;
    }

    public bool IsOblique()
    {
        return m_IsOblique;
    }

    public string GetState()
    {
        return m_State;
    }
}
