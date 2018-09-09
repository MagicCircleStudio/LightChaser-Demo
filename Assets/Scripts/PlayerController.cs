using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject floorDetectorPrefab;
    [HideInInspector] public GameObject floorDetector;

    public Transform bulletParent;
    public float fireCooldown = 2f;

    public float maxVelocity = 3f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bulletParent = GameObject.Find("Flying Objects").transform;
        if (bulletParent == null)
            bulletParent = Instantiate(new GameObject("Flying Objects")).transform;
        floorDetector = Instantiate(floorDetectorPrefab, transform.parent);

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

            rb.velocity = new Vector3(h, 0, v) * k * maxVelocity + new Vector3(0, rb.velocity.y, 0);
        }

        if (Input.GetAxis("Fire1") > 0)
        {
            var bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity, bulletParent);
        }

        floorDetector.transform.position = new Vector3(0, transform.position.y, 0);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 4000, ~(1 << LayerMask.GetMask("Only Raycast"))))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            transform.LookAt(hit.point);
        }
    }
}
