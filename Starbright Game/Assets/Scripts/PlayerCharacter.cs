using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

	public static PlayerCharacter instance;

	public float maxMass;

	private bool isOrbiting;
	private bool isColliding;
	private Body body; //body that it's orbiting

	public bool inBounds = true;

	private Vector3 lastPosition;
	private Vector3 deltaPosition;
	GameObject camera;

	private float targetMass;

	public float MaxMass 
	{
		get
		{
			return maxMass;
		}
	}

	public float Mass 
	{
		get 
		{
			return BodyComponent.Mass;
		}
	}

	public Body BodyComponent
	{
		get
		{
			return GetComponent<Body>();
		}
	}

	public bool IsOrbiting() {
		return isOrbiting;
	}

	public bool IsColliding() {
		return isColliding;
	}

	// Use this for initialization
	void Start () {
		instance = this;
		isOrbiting = false;
		isColliding = true;

		lastPosition = transform.position;
		camera = GameObject.Find ("Main Camera");

		targetMass = 40f;
	}
	
	// Update is called once per frame
	void Update () {

		if (isOrbiting) {
			GetComponent<Body>().Gravitiate (body);
		}

		if(isColliding) {
			isColliding = !isColliding;
		}

		deltaPosition = transform.position - lastPosition;

	
		if(Mass > targetMass)
		{
			isOrbiting = false;
			Generator.instance.GetComponent<Generator>().LayerUp ();
			targetMass = targetMass * (targetMass/2f);
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
		Debug.Log (c.gameObject.transform.position + " THERE WAS A COLLISION");
		Debug.Log (c.GetType());
		if(!isColliding)
		{
			BroadcastMessage ("Hit", c.gameObject.GetComponent<Body> ());
			isColliding = true;
		}
	}

	public Vector3 getDeltaPosition()
	{
		return deltaPosition;
	}



}
