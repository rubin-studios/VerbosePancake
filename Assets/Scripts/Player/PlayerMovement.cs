using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 20f;

    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody2D;


    private float horizontalMove = 0f;
    private bool jump = false;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {   
        m_Animator.SetBool("Jump", jump); 
        m_Animator.SetBool("Grounded", controller.IsGrounded());
        m_Animator.SetFloat("VelocityY", m_Rigidbody2D.velocity.y);
        m_Animator.SetFloat("Horizontal", horizontalMove);
        m_Animator.SetFloat("VelocityX", Mathf.Abs(m_Rigidbody2D.velocity.x));

        // Move
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
