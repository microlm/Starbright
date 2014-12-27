using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

	public static PlayerCharacter instance;

	public float maxMass;

	private bool isOrbiting;
	private bool isColliding;
	private Body body; //body that it's orbiting
	private Body playerBody;
	private bool gameOver;

	public bool inBounds = true;
	bool flashed = false;

	private Vector3 lastPosition;
	private Vector3 deltaPosition;
	GameObject camera;
	GameObject backgroundCamera;

	FlashBehavior flash;
	private float targetMass;
	private float downMass;

	bool disabled = false;


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
		set
		{
			BodyComponent.Mass = value;
		}
	}

	public Body BodyComponent
	{
		get
		{
			return GetComponent<Body>();
		}
	}

	public bool IsOrbiting() 
	{
		return isOrbiting;
	}

	public bool IsColliding() 
	{
		return isColliding;
	}

	// Use this for initialization
	void Start () 
	{
		instance = this;
		isOrbiting = false;
		isColliding = true;

		lastPosition = transform.position;
		camera = GameObject.Find ("Main Camera");
		backgroundCamera = GameObject.Find ("Background Planets Camera");

		flash = GameObject.Find ("Flash").GetComponent<FlashBehavior>();
		downMass = 4;
		targetMass = ProgressCircle.instance.TargetSize; 
		playerBody = GetComponent<Body>();

		DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		BodyComponent.UpdateBody ();

		if(!gameOver)
		{
			if (isOrbiting) {
				GetComponent<Body>().Gravitiate (body);
			}

			if(isColliding) {
				isColliding = !isColliding;
			}

			deltaPosition = transform.position - lastPosition;

			if(Mass >= targetMass)
			{
				isOrbiting = false;
				LevelUp ();
			}
			else if(Mass < downMass)
			{
				isOrbiting = false;
				LevelDown();
			}

			else if(disabled)
			{
				disabled = false;
			}

			if(Input.GetKeyDown(KeyCode.Q))
			{
				GameOver ();
			}
		}
		else
		{
			if(flash.blackFinished)
			{
				Application.LoadLevel("FinalScore");
			}
		}
	}

	public void Orbit (Body b) {
		if (b != this.BodyComponent) {
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
	
	public Vector3 getDeltaPosition()
	{
		return deltaPosition;
	}

	public void LevelUp()
	{
		if(!flash.getWhiteFlash () && !disabled)
		{
			flash.whiteFlash();

			disabled = true;
			GameObject.Find ("Trail").GetComponent<TrailRenderer>().enabled = false;
			GetComponent<CircleCollider2D>().enabled = false;
			GetComponent<Body>().enabled = false;
			Generator.instance.GetComponent<Generator>().LayerUp ();
			transform.position = backgroundCamera.transform.position;
			camera.transform.position = transform.position;
		}
	
		
		if(!flash.getWhiteFlash () && disabled)
		{
			GameObject.Find ("Trail").GetComponent<TrailRenderer>().enabled = true;
			targetMass = ProgressCircle.instance.TargetSize;
			BodyComponent.enabled = true;

			GetComponent<CircleCollider2D>().enabled = true;

			disabled = false;
		}
		isOrbiting = false;
	}
	
	public void LevelDown()
	{
		
		if(!flash.getBlackFlash () && !disabled)
		{
			flash.blackFlash();
			Generator.instance.GetComponent<Generator>().LayerDown ();
			
			transform.position = camera.transform.position;
		}
		
		if(flash.getBlackFlash () && !disabled)
		{
			disabled = true;
			GameObject.Find ("Trail").GetComponent<TrailRenderer>().enabled = false;
			GetComponent<Body>().enabled = false;
		}
		
		if(!flash.getBlackFlash ())
		{
			GameObject.Find ("Trail").GetComponent<TrailRenderer>().enabled = true;
			GetComponent<Body>().enabled = true;
			downMass = downMass/2f;
		}
		isOrbiting = false;
	}

	public void GameOver()
	{
		flash.blackScreen();
		gameOver = true;

		GetComponent<CircleCollider2D>().enabled = false;
	}
}
