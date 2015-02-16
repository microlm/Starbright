using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {
	
	public static float elasticity = 0.3f;
	public static float orbitalStrength = 2f;
	
	private float G = .0001f;
	private float growth = .1f;

	public float mass;
	public Vector2 initialVel;

	private Vector2 velocity;
	private float maxSpeed = .15f;
	private float minSpeed = 0.005f;
	
	private Vector3 maxScale;
	private Vector3 cMaxScale;
	
	GameObject camera;
		
	public float Mass 
	{
		get { return mass; }
		set { mass = value; }
	}
	
	public Vector3 Position 
	{
		get { return gameObject.transform.position; }
		set {
			//set value and lock z
			float z = gameObject.transform.position.z;
			gameObject.transform.position = new Vector3(value.x, value.y, z);
		}
	}

	public Vector2 Velocity
	{
		get { return velocity; }
	}

	public Vector3 Scale
	{
		get { return gameObject.transform.localScale; }
		set { transform.localScale = value; }
	}

	public Quaternion Rotation
	{
		get { return gameObject.transform.rotation; }
		set { gameObject.transform.rotation = value; }
	}

	/** The Glow gameObject that is attached to Body*/
	public Glow GlowChild
	{
		get { return gameObject.GetComponentsInChildren<Glow>()[0]; }
	}

	public Color BodyColor
	{
		get { return GetComponent<SpriteRenderer> ().color; }
		set { GetComponent<SpriteRenderer> ().color = value; }
	}
	
	void Start () {
		velocity = initialVel;
		maxScale = Scale;
		cMaxScale = gameObject.collider2D.transform.localScale;

		camera = GameObject.Find ("Main Camera");
	}
	
	void Update () {
		if (Game.Instance.State == GameState.Playing) 
		{
			Scale = ScaleByMass ();
			BodyColor = ColorOption.Instance.assignColor(Mass);

			if (PlayerCharacter.instance != null && !GlowChild.InTransisition && !GlowChild.IsActive) 
			{
				if(Mass <= PlayerCharacter.instance.Mass)
				{
					GlowChild.DefaultColor = Color.white;
				}
				else if(Mass > PlayerCharacter.instance.Mass)
				{
					GlowChild.DefaultColor = BodyColor;
				}
			}
		}
	}


	Vector3 ScaleByMass()
	{
		return maxScale / 20 * Mass;
	}

	/** Updates speed, position, size, and color of Body */
	public void UpdateBody()
	{
		//check speed
		while (Velocity.magnitude > Mass*maxSpeed)
			velocity *= .95f;
		
		//move
		Position += new Vector3(Velocity.x * Time.deltaTime * 100, Velocity.y * Time.deltaTime * 100, 0);
		
		//correct size
		Scale = Vector3.Lerp (Scale, ScaleByMass(), Time.deltaTime);
		collider2D.transform.localScale = cMaxScale / 20 * Mass;
		
		//update color
		BodyColor = ColorOption.Instance.assignColor(Mass);
	}
	
	void OnMouseDown () 
	{
		//playor orbits this body
		PlayerCharacter.instance.Orbit (this);
		GlowChild.Activate ();
		TutorialController.Instance.ShowEvent ("tap and hold");
	}
	
	void OnMouseUp () 
	{
		//player stops orbiting this body
		PlayerCharacter.instance.StopOrbit ();
		GlowChild.Deactivate ();
	}

	public void ResetVelocity()
	{
		velocity = Vector3.zero;
	}

	public void Hit(Body b)
	{
		if (Mass >= b.Mass) 
		{
			Eat (b);

			ScoreManager.Instance.AddScore(Mathf.FloorToInt(b.Mass));

			if (!TutorialController.Instance.ShowEvent ("eating a planet 1")) {
				if (!TutorialController.Instance.ShowEvent ("eating a planet 2"))
					TutorialController.Instance.ShowEvent ("eating a planet 3");
			}
		}
		else
		{
			CameraBehavior.Instance.Shake(Velocity.magnitude / maxSpeed * 5f);
			ScoreManager.Instance.ResetMultiplier();

			if (Mass > 1.5) {
				Shrink (b);
			}

			TutorialController.Instance.ShowEvent ("hit a large planet");
		}
		
	}

	/** "Eats" Body b. Mass grows, velocity increases, stops orbiting b, causes an explosion, and sets b to inactive */
	public void Eat(Body b)
	{
		SoundManager.Instance.PlayEatSound (SoundManager.Instance.GetNoteByMass(b.Mass));

		Mass += growth * b.Mass;
		velocity += growth * Velocity * b.Mass / Mass;
		if(PlayerCharacter.instance.getOrbiting() == b)
		{
			PlayerCharacter.instance.StopOrbit();
		}
		float massLost = Mass * Velocity.magnitude;
		GetComponent<Explosion>().Explode(massLost, Mass, Velocity);
		b.gameObject.SetActive(false);
	}

	/** causes damage proportional to size of body b and how fast was going and stop orbit around b */
	public void Shrink(Body b)
	{
		SoundManager.Instance.PlayHitSound (SoundManager.Instance.GetNoteByMass(b.Mass));

		float massLost = Mass * Velocity.magnitude;
		Mass -= massLost;
		velocity = setExitVelocity(b);
		GetComponent<Explosion>().Explode(massLost, Mass, Velocity);
		
		PlayerCharacter.instance.StopOrbit();
	}
	
	/** Adjusts velocity to mimic accleration due to gravity cause by Body */
	public void Gravitiate(Body b) {
		float dist = Vector2.Distance (b.gameObject.transform.position, gameObject.transform.position);
		Vector2 r = b.gameObject.transform.position - gameObject.transform.position;
		
		velocity += (G * Mass * dist / Mathf.Pow (dist, orbitalStrength) * r);
	}

	/** Adjusts velocity to mimic accleration due to gravity cause by BlackHole. 
	 * float modifier adjusts strength of gravity and is optional */
	public void Gravitiate(BlackHole b, float modifier = 1f) {
		float dist = Vector2.Distance (b.Position, gameObject.transform.position);
		Vector2 r = b.Position - gameObject.transform.position;
		
		velocity += (G * Mass * dist * modifier / Mathf.Pow (dist, orbitalStrength) * r);
	}

	public Vector2 setExitVelocity(Body b)
	{
		Vector2 unitNormal = new Vector2(Position.x - b.Position.x, Position.y - b.Position.y);
		unitNormal = unitNormal / Mathf.Pow((Mathf.Pow (unitNormal.x, 2) + Mathf.Pow (unitNormal.y, 2)), 0.5f);
		
		Vector2 unitTangent = new Vector2(-unitNormal.y, unitNormal.x);
		
		float intVelNormal = Vector2.Dot(unitNormal, velocity);
		float intVelTangent = Vector2.Dot (unitTangent, velocity);
		
		float finVelTangent = intVelTangent;
		float finVelNormal = (intVelNormal*(Mass - b.Mass))/(Mass + b.Mass);
		
		Vector2 finalTangent = finVelTangent*unitTangent;
		Vector2 finalNormal = finVelNormal*unitNormal;
		
		return finalNormal + finalTangent;
	}
}
