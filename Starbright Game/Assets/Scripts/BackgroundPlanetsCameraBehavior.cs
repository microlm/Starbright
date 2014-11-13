using UnityEngine;
using System.Collections;

public class BackgroundPlanetsCameraBehavior : MonoBehaviour {

	public Camera main;
	private float factor;
	public GameObject pc;

	private Vector3 previousPosition;
	private float previousCamSize;
	private Vector3 deltaPosition;


	private float targetMass;
	// Use this for initialization
	void Start () {

		previousPosition = main.camera.transform.position;
		previousCamSize = main.camera.orthographicSize;

		camera.transform.position = previousPosition;
		camera.orthographicSize = previousCamSize;
		targetMass = 40f;
	}
	
	// Update is called once per frame
	void Update () {

		factor = pc.GetComponent<Body>().Mass/targetMass;

		deltaPosition = (main.camera.transform.position - previousPosition)*factor;

		camera.transform.position = new Vector3(camera.transform.position.x + deltaPosition.x, camera.transform.position.y + deltaPosition.y, camera.transform.position.z);
		camera.orthographicSize += (main.camera.orthographicSize - previousCamSize) * factor;

		previousPosition = main.camera.transform.position;
		previousCamSize = main.camera.orthographicSize;
	}

	public void setTargetMass(int mass)
	{
		targetMass = mass;
	}
}
