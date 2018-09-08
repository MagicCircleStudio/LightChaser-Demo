using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float maxVelocity = 3f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            var k = 1f; // 将正方形分布的方向矢量映射成圆形分布的系数，防止斜角速度过快

            if (Mathf.Abs(v) > Mathf.Abs(h))
                k = 1 / new Vector2(h, 1).magnitude;
            else
                k = 1 / new Vector2(v, 1).magnitude;

            rb.velocity = new Vector3(h, 0, v) * k * maxVelocity;
        }
    }
}
