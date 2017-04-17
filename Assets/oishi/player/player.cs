using UnityEngine;
using System;

public class player : MonoBehaviour
{
    CharacterController m_chara;

    public float speed;
    public float jumpPower;
    public float gravity;
    public GameObject camera;

    Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        m_chara = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (m_chara.isGrounded)
        {
            moveDirection = Quaternion.Euler(0, camera.transform.localEulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);

            moveDirection *= speed;

            if (Input.GetButton("Jump")) moveDirection.y = jumpPower;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        m_chara.Move(moveDirection * Time.deltaTime);
    }
}