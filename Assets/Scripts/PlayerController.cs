using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject floorDetectorPrefab;
    public CinemachineVirtualCamera cam;
    private Transform noRotator;
    [HideInInspector] public GameObject floorDetector;

    public Transform bulletParent;
    public float fireCooldown = 0.4f;
    private float fireCounter = 0f;

    public float maxVelocity = 3f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bulletParent = GameObject.Find("Flying Objects").transform;
        if (bulletParent == null)
            bulletParent = Instantiate(new GameObject("Flying Objects")).transform;
        floorDetector = Instantiate(floorDetectorPrefab, transform.parent);

        noRotator = Instantiate(new GameObject("No Rotator"), transform.parent).transform;

        cam.Follow = noRotator;
        cam.LookAt = transform;
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

        fireCounter += Time.deltaTime;
        if (Input.GetAxis("Fire1") > 0 && fireCounter >= fireCooldown)
        {
            var bullet = Instantiate(bulletPrefab, transform.position + transform.forward * 0.5f + new Vector3(0, 0.5f, 0), transform.rotation, bulletParent);
            fireCounter = 0;
        }

        floorDetector.transform.position = new Vector3(0, transform.position.y, 0);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 lookAtAhead = new Vector3();
        if (Physics.Raycast(ray, out hit, 4000, ~(1 << LayerMask.GetMask("Only Raycast"))))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            Vector3 lookAt = hit.point;
            lookAt.y = transform.position.y;
            transform.LookAt(lookAt);
            Vector3 aiming = lookAt - transform.position;
            lookAtAhead = aiming.normalized * Mathf.Clamp(aiming.magnitude / 10, 0, 1);
        }

        noRotator.position = transform.position - lookAtAhead * 3;
    }
}
