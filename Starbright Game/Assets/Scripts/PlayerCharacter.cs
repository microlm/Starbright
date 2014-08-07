using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

	public float maxMass;

	private bool isOrbiting;
	private bool isColliding;
	private Body body; //body that it's orbiting

	public bool inBounds = true;
	
	GameObject camera;

	public float MaxMass () {
		return maxMass;
	}

	public float Mass () {
		return GetComponent<Body> ().Mass ();
	}

	public bool IsOrbiting() {
		return isOrbiting;
	}

	public bool IsColliding() {
		return isColliding;
	}

	// Use this for initialization
	void Start () {
		isOrbiting = false;
		isColliding = true;

		camera = GameObject.Find ("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {

		if (isOrbiting) {
			GetComponent<Body>().Gravitiate (body);
		}

		if(isColliding) {
			isColliding = !isColliding;
		}

		//Turn these off when you're not using them!
		//Debug.Log(GetComponent<Body>().getVelocity().magnitude * 10f);
	}

	void Orbit (Body b) {
		if (b != this.GetComponent<Body>()) {
			isOrbiting = true;
			body = b;
		}
	}

	public void StopOrbit() {
		isOrbiting = false;
		camera.BroadcastMessage("CameraReturn", this);
	}

	public Body getOrbiting() {
		return body;
	}

	void OnCollisionEnter2D(Collision2D c) {
		if(!isColliding)
		{
			BroadcastMessage ("Hit", c.gameObject.GetComponent<Body> ());
			isColliding = true;
		}
	}

}
