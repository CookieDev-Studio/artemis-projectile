using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyBullet : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 300;

    }

    // Update is called once per frame
    void Update()
    {
    }
}
