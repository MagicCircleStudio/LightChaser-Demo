using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeSpan = 5f;
    public float flyHeight = 0;

    private void Start()
    {
        Destroy(gameObject, lifeSpan);
        flyHeight = transform.position.y;
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        var v = transform.position;
        transform.position = new Vector3(v.x, flyHeight, v.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log(collision.gameObject.name);
        var normalVector = collision.contacts[0].normal;
        var outVector = transform.forward + normalVector * 2;
        transform.LookAt(transform.position + outVector);
    }

}
