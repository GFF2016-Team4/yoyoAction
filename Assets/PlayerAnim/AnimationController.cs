using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    [SerializeField, HideInInspector]
    public float m_Speed;
    [SerializeField, HideInInspector]
    private Animator m_Animator;


    private bool m_IsGround;
    private bool m_IsJumped;
    private int m_RailState;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        m_Speed = transform.GetComponent<Player>().PlayerSpeed;
        m_Animator = transform.GetComponent<Animator>();
        m_IsGround = transform.GetComponent<Player>().PlayerIsGround;

        if (m_IsGround == false && Input.GetKey(KeyCode.Space))
        {
            m_IsJumped = true;
        }
        else if(m_IsGround == true)
        {
            m_IsJumped = false;
        }

        if(Input.GetMouseButton(0))
        {
            m_Animator.SetBool("IsShoot", true);
        }
        else
        {
            m_Animator.SetBool("IsShoot", false);
        }

        //if(transform.GetComponent<Player>().hitInfo.collider.tag == "Rail")
        //{
        //    string state = transform.GetComponent<Player>().hitInfo.collider.GetComponent<RailController>().GetState();

        //    if(state == "front" || state == "back")
        //    {
        //        m_RailState = 0;
        //    }
        //    else if(state == "left" || state == "right")
        //    {
        //        m_RailState = 1;
        //    }
        //}

        m_Animator.SetInteger("RailState", m_RailState);
        m_Animator.SetFloat("Speed", m_Speed);
        m_Animator.SetBool("IsGround", m_IsGround);
        m_Animator.SetBool("IsJumped", m_IsJumped);
	}
}
