using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public GrapplingHook hook;
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

        if (Input.GetButtonDown("Fire2"))
        {
           hook.Anchor();
        }
        else 
        {
            if (Input.GetButton("Fire2")) 
            {
                hook.OnGrapple(horizontalMove);
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                hook.Release();
            }
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
        if (!hook.IsGrappling())
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        // controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump, hook.IsGrappling());

        jump = false;
    }
}
