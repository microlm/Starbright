using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

	public static PlayerCharacter instance;

	private bool isOrbiting;
	private bool isColliding;
	private Body body; //body that it's orbiting
	private bool gameOver;

	public bool inBounds = true;
	bool flashed = false;

	private Vector3 lastPosition;
	private Vector3 deltaPosition;
	GameObject camera;
	GameObject backgroundCamera;

	FlashBehavior flash;

	private float initialMass;

	public float Mass 
	{
		get { return BodyComponent.Mass; }
		set { BodyComponent.Mass = value; }
	}

	public Body BodyComponent
	{
		get { return GetComponent<Body>(); }
	}

	public Vector3 Position
	{
		get { return BodyComponent.Position; }
		set { BodyComponent.Position = value; }
	}

	public TrailRenderer Trail
	{
		get { return GetComponentInChildren<TrailRenderer> (); }
	}

	public CircleCollider2D CircleCollider
	{
		get { return GetComponent<CircleCollider2D> (); }
	}

	public bool IsOrbiting() 
	{
		return isOrbiting;
	}

	public bool IsColliding() 
	{
		return isColliding;
	}

	public Vector3 getDeltaPosition()
	{
		return deltaPosition;
	}

	public void setOrbiting(bool o)
	{
		isOrbiting = o;
	}
	
	void Start () 
	{
		instance = this;
		isOrbiting = false;
		isColliding = true;

		lastPosition = transform.position;
		camera = GameObject.Find ("Main Camera");
		backgroundCamera = GameObject.Find ("Background Planets Camera");
		initialMass = BodyComponent.Mass;

		flash = GameObject.Find ("Flash").GetComponent<FlashBehavior>();

		DontDestroyOnLoad (this);

		TutorialController.Instance.ShowEvent("tap a planet");
	}

	void Update () 
	{
		if (Game.Instance.State == GameState.Playing)
		{
			BodyComponent.UpdateBody ();

			if(!gameOver)
			{
				if (isOrbiting) {
					BodyComponent.Gravitiate (body);
				}

				if(isColliding) {
					isColliding = !isColliding;
				}

				deltaPosition = transform.position - lastPosition;

				if(Input.GetKeyDown(KeyCode.Q))
				{
					GameOver ();
				}
			}
			else
			{
				if(flash.blackFinished)
				{
					Game.Instance.EndGame("FinalScore");
				}
			}

			BodyComponent.GlowChild.CurrentColor = BodyComponent.BodyColor;
		}
	}

	void OnCollisionEnter2D(Collision2D c) {
		if(!isColliding)
		{
			Body b = c.gameObject.GetComponent<Body> ();
			if (b != null) {
				BodyComponent.Hit(b);
				isColliding = true;
			}
		}
	}

	public void Orbit (Body b) {
		if (b != BodyComponent) {
			isOrbiting = true;
			body = b;
		}
	}

	public void StopOrbit() {
		isOrbiting = false;
		camera.GetComponent<CameraBehavior> ().CameraReturn ();
	}

	public Body getOrbiting() {
		return body;
	}

	public void LevelRefresh()
	{
		BodyComponent.ResetVelocity ();
		BodyComponent.Mass = initialMass;
		BodyComponent.UpdateBody ();
		Generator.instance.LevelRefresh ();
		ColorOption.Instance.GenerateNewGradiant ();
	}

	public void GameOver()
	{
		GetComponent<CircleCollider2D>().enabled = false;
		GetComponent<Explosion>().Explode(Mass, Mass, BodyComponent.Velocity * 10f);
		flash.blackScreen();
		gameOver = true;
	}

	public void Restart()
	{
		Destroy (this);
		Game.Instance.Start (Application.loadedLevelName);
	}
}
