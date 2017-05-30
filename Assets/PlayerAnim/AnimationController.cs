using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    [SerializeField, HideInInspector]
    public float m_Speed;
    [SerializeField]
    private Animator m_Animator;


    private bool m_IsGround;
    private bool m_IsJumped;

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

        m_Animator.SetFloat("Speed", m_Speed);
        m_Animator.SetBool("IsGround", m_IsGround);
        m_Animator.SetBool("IsJumped", m_IsJumped);
	}
}
