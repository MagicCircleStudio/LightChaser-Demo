using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour {

	public PlayerController player;

	void Start() {
		if (player == null)
			player = gameObject.GetComponentInParent<PlayerController>();

	}

	Vector3 YRotationToDirection(float y) {
		y = y % 360;
		var posZ = Mathf.Cos(y / 180 * Mathf.PI);
		var posX = Mathf.Sin(y / 180 * Mathf.PI);
		return new Vector3(posX, 0, posZ);
	}

	// Update is called once per frame
	void Update() {
		transform.position = player.transform.position + 0.5f * YRotationToDirection(player.transform.eulerAngles.y);
	}
}