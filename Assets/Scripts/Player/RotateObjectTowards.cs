using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjectTowards : MonoBehaviour
{

    public float rotationSpeed = 10f;
    public Transform firePoint;

    public GameObject fireballPrefab;
    
    // Update is called once per frame
    void Update()
    {

        // find the mouse and calculate the angle
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        //rotate the object
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        //if click fire, fire a fireball
        if (Input.GetButtonDown("Fire1"))
        {
            ShootFireball();
        }
    }

    void ShootFireball ()
    {
        Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
    }
}
