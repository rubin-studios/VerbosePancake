using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballMovement : MonoBehaviour
{
    public float bulletSpeed = 6f;
    private Vector3 m_Velocity = Vector3.zero;
    public Rigidbody2D myRigidBody;
    private Rigidbody2D playerRigidBody;
    public Collider2D selfCollider;

    // Start is called before the first frame update
    void Start()
    {
        //ignore collisions with self
        GameObject player = GameObject.Find("Player");
        Physics2D.IgnoreCollision(selfCollider, player.GetComponent<BoxCollider2D>());
        Physics2D.IgnoreCollision(selfCollider, player.GetComponent<CircleCollider2D>());

        // add recoil to the player
        playerRigidBody = player.GetComponent<Rigidbody2D>();
        Vector2 v = transform.right * bulletSpeed;
        v = v * -15f;
        playerRigidBody.AddForce(v);

    }

    private void FixedUpdate()
    {
        // calculate fireball trajectory
        myRigidBody.velocity = Vector3.SmoothDamp(myRigidBody.velocity, transform.right * bulletSpeed, ref m_Velocity, 2f);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Destroy(gameObject);
    }
}
