using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public float distance;
    public float step;
    public float force;
    public LineRenderer line;
    public LayerMask mask;

    private DistanceJoint2D dj2d;
    private Rigidbody2D rb2d;
    private Vector3 targetPos;
    private RaycastHit2D hit;
    private bool m_Grappling = false;
    private float horizontalMove = 0f;


    // Start is called before the first frame update
    void Awake()
    {
        dj2d = GetComponent<DistanceJoint2D>();
        rb2d = GetComponent<Rigidbody2D>();

        dj2d.enabled = false;
        line.enabled = false;
        m_Grappling = false;
    }

    private void FixedUpdate() {
        if (dj2d.distance > 1f) 
        {
            dj2d.distance -= step;
        }
        
        if (m_Grappling) 
        {
            // get normalized vector perpendicular to hook point
            Vector2 forceDirection = Vector2.Perpendicular(dj2d.connectedAnchor - (Vector2) transform.position);
            forceDirection.Normalize();

            // apply force
            if (horizontalMove < 0.1f) 
            {
                 rb2d.AddForce(forceDirection * force);
            }
            else if (horizontalMove > 0.1f)
            {
                rb2d.AddForce(-forceDirection * force);
            }
        }
    }

    // Update is called once per frame
    public void Anchor()
    {   
        targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = 0.0f;

        hit = Physics2D.Raycast(transform.position, targetPos - transform.position, distance, mask);
    
        // Checks that the object hit is a rigid body
        if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            m_Grappling = true;
            dj2d.enabled = true;

            Vector2 connectPoint = hit.point - new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y);
            connectPoint.x = connectPoint.x / hit.collider.transform.localScale.x;
            connectPoint.y = connectPoint.y / hit.collider.transform.localScale.y;

            dj2d.connectedAnchor = connectPoint;
            dj2d.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
            dj2d.distance = Vector2.Distance(transform.position, hit.point);

            line.enabled = true;
            line.SetPosition(0,transform.position);
            line.SetPosition(1,hit.point);
        }
    }

        
    public void OnGrapple(float horizontalInput) 
    {   
        horizontalMove = horizontalInput;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, dj2d.connectedBody.transform.TransformPoint(dj2d.connectedAnchor));
        dj2d.distance = Vector2.Distance(transform.position, hit.point);
    }


    public void Release()
    {
        m_Grappling = false;
        dj2d.enabled = false;
        line.enabled = false;
    }

    public bool IsGrappling()
    {
        return m_Grappling;
    }
}
