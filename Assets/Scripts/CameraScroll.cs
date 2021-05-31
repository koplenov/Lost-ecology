using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    private Vector3 normalizeDirection;

    public Transform start;

    public Transform target;

    public float speed = 5f;

    void Start()
    {
        transform.position = new Vector3(start.position.x, start.position.y, - 10);

        normalizeDirection = (new Vector3(target.position.x, target.position.y, - 10) - transform.position).normalized;
    }

    void FixedUpdate()
    {
        if (transform.position.x > -67f)
        {
            transform.position += normalizeDirection * speed * Time.deltaTime;           
        }
        else
        {
            transform.position = new Vector3(start.position.x, start.position.y, -10);
        }
    }
}
