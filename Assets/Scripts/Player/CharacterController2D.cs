using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 40f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, 2f)] [SerializeField] private float m_MovementSmoothing = 1f;	// How much to smooth out the movement
	[SerializeField] public bool m_AirControl = true;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .06f; // Radius of the overlap circle to determine if grounded
    public float wallCheckDistance = 1f;
	private bool m_Grounded;            // Whether or not the player is grounded.
    private bool doubleJump;
    private bool onWallFront;
    private bool onWallBack;
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasSliding = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
        bool wasOnWallBack = onWallBack;
        bool wasOnWallFront = onWallFront;
        bool wasGrounded = m_Grounded;
        onWallBack = false;
        onWallFront = false;
        m_Grounded = false;

        //Using raycasts left and right to detect if touching a wall
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D m_FrontWallCheck = Physics2D.Raycast(m_GroundCheck.position, Vector2.right * transform.localScale.x, wallCheckDistance);
        RaycastHit2D m_BackWallCheck = Physics2D.Raycast(m_GroundCheck.position, Vector2.left * transform.localScale.x, wallCheckDistance);

        if (m_BackWallCheck.collider!=null)
        {
            onWallBack = true;
            if(!wasOnWallBack)
                doubleJump = true;
        }
        if (m_FrontWallCheck.collider != null)
        {
            onWallFront = true;
            if(!wasOnWallFront)
            {
                doubleJump = true;
                Flip();
            }
                

        }

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                    doubleJump = true;
                }
			}
		}
	}

    void OnDrawGizmos ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_GroundCheck.position, m_GroundCheck.position + Vector3.left * transform.localScale.x * wallCheckDistance);
    }

	public void Move(float move, bool slide, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!slide)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				slide = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (slide)
			{
				if (!m_wasSliding)
				{
					m_wasSliding = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasSliding)
				{
					m_wasSliding = false;
					OnCrouchEvent.Invoke(false);
				}
			}

            float smoothing = m_MovementSmoothing;
            //if in the air, less acceleration
            if (!m_Grounded)
                smoothing = m_MovementSmoothing * 4;
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, smoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
        // If the player should walljump...
        if (!m_Grounded && (onWallFront || onWallBack) && jump)
        {
            // Add a vertical and horizontal force away from the wall to the player
            if (!m_Grounded && onWallBack)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * 2f, 3f);
                onWallBack = false;
            }
            else if (!m_Grounded && onWallFront)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * -2f, 3f);
                onWallFront = false;
            }

        }

        // If the player should jump...
        else if ((m_Grounded||doubleJump) && jump)
		{
            // Add a vertical force to the player.
            if (!m_Grounded)
            {
                doubleJump = false;
                Vector2 v = GetComponent<Rigidbody2D>().velocity;

                //This code adds horizontal force for double jumping

                //if (move < 0 && v.x > 0)
                //    m_Rigidbody2D.AddForce(new Vector2(m_JumpForce*-1f, 0f));
                //if (move > 0 && v.x < 0)
                //    m_Rigidbody2D.AddForce(new Vector2(m_JumpForce*1f, 0f));
               
                v.y = 3f;
                GetComponent<Rigidbody2D>().velocity = v;
            }

            else
            {
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
		}
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	// Public Getter Methods
	public bool IsGrounded() {
		return m_Grounded;
	}
}
