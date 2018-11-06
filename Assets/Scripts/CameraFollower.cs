using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

	public GameObject followingObject;
	float height;

	void Start() {
		height = transform.position.y - followingObject.transform.position.y;
	}
	
	void Update () {
		if (followingObject) {
			transform.position = followingObject.transform.position + height * Vector3.up;
		}
	}
}
