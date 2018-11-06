using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour {

	public float floatingSpeed = 1f;
	public float floatingRange = 1f;
	public Color lightColor;
	public GameObject vision;

	private GameObject sphere;
	private Renderer rendererCom;
	private Light lightCom;
	private Material material;
	private Vector3 originPosition;

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			Debug.Log("Player");
			vision.SetActive(true);
		}
	}

	private void Start() {
		sphere = transform.GetChild(0).gameObject;
		rendererCom = sphere.GetComponent<Renderer>();
		lightCom = sphere.GetComponent<Light>();
		material = rendererCom.material;
		originPosition = transform.position;

		material.SetColor("_EmissionColor", lightColor);
		lightCom.color = lightColor;
	}

	private float timeCounter = 0;
	private void Update() {
		timeCounter += Time.deltaTime;
		transform.position = originPosition +
			(transform.up * Mathf.Cos(timeCounter * floatingSpeed * Mathf.PI)) * floatingRange / 2;
	}
}