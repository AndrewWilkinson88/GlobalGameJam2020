using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepInPlay : MonoBehaviour
{
    static Vector3 WORLD_LIMITS = new Vector3(15, -1, 15);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < WORLD_LIMITS.y)
        {
            transform.position = new Vector3(0, 2, 0);
            ZeroVelocity();
        }
        if (transform.position.x > WORLD_LIMITS.x)
        {
            transform.position = new Vector3(WORLD_LIMITS.x, transform.position.y, transform.position.z);
            ZeroVelocity();
        }
        else if(transform.position.x < -WORLD_LIMITS.x)
        {
            transform.position = new Vector3(-WORLD_LIMITS.x, transform.position.y, transform.position.z);
            ZeroVelocity();
        }
        if (transform.position.z > WORLD_LIMITS.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, WORLD_LIMITS.z);
            ZeroVelocity();
        }
        else if(transform.position.z < -WORLD_LIMITS.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -WORLD_LIMITS.z);
            ZeroVelocity();
        }
    }

    void ZeroVelocity()
    {
        Rigidbody r = GetComponent<Rigidbody>();
        if (r)
        {
            r.velocity = Vector3.zero;
        }
    }
}
