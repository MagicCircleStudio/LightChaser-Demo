using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 5f;
	public float rotateSpeed = 1f;
	[HideInInspector] public Vector3 moveDirection;
	[HideInInspector] public Vector3 faceDirection;

	public GameObject bulletPrefab;
	public Transform bulletParent;
	public Transform bulletSpawnPosition;
	public float fireCooldown = 0.4f;
	private float fireCounter = 0f;

	private void Start() {
		var parent = GameObject.Find("Bullet Parent");
		if (parent == null)
			bulletParent = new GameObject("Bullet Parent").transform;
		else
			bulletParent = parent.transform;
	}

	private void Update() {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		moveDirection = Vector3.Normalize(new Vector3(horizontal, 0f, vertical));

		if (moveDirection.magnitude > 0.64f)
			faceDirection = moveDirection;
		else
			faceDirection = transform.forward;

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(faceDirection), rotateSpeed * Time.deltaTime);
		transform.position += moveDirection * moveSpeed * Time.deltaTime;

		fireCounter += Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.Space) && fireCounter >= fireCooldown) {
			Instantiate(bulletPrefab, bulletSpawnPosition.position, bulletSpawnPosition.rotation, bulletParent);
			fireCounter = 0;
		}
	}
}