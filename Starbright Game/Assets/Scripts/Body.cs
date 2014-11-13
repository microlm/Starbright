using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {
	
	public static float elasticity = 0.3f;
	public static float orbitalStrength = 2f;
	
	private float G = .0001f;
	private float growth = .1f;
	
	private Vector2 velocity;
	public float mass;
	public Vector2 initialVel;
	private float maxSpeed = .15f;
	private float minSpeed = 0.005f;
	
	private Vector3 maxScale;
	private Vector3 cMaxScale;
	
	GameObject pc;
	
	ColorOption colorOpt;
	GameObject camera;
	ScoreManager score;
		
	public float Mass 
	{
		get 
		{
			return mass;
		}
	}
	
	public Vector3 Position 
	{
		get 
		{ 
			return gameObject.transform.position; 
		}
		set 
		{
			//set value and lock z
			float z = gameObject.transform.position.z;
			gameObject.transform.position = new Vector3(value.x, value.y, z);
		}
	}
	
	// Use this for initialization
	void Start () {
		velocity = initialVel;
		maxScale = gameObject.transform.localScale;
		cMaxScale = gameObject.collider2D.transform.localScale;
		
		gameObject.transform.localScale = maxScale / 20 * mass;
		colorOpt = GameObject.Find ("ColorOptions").GetComponent<ColorOption> ();
		GetComponent<SpriteRenderer>().color = colorOpt.assignColor(mass);
		
		pc = GameObject.Find("PC");
		camera = GameObject.Find ("Main Camera");
		score = GameObject.Find ("ScoreObject").GetComponent<ScoreManager> ();
	}
	
	public void Hit(Body b)
	{
		Debug.Log ("hiiit");
		if (mass >= b.mass) 
		{
			mass += growth * b.mass;
			velocity += growth*velocity*b.mass/mass;
			if(pc.GetComponent<PlayerCharacter>().getOrbiting() == b)
			{
				pc.BroadcastMessage("StopOrbit", this);
			}
			float massLost = mass * velocity.magnitude;
			GetComponent<Explosion>().Explode(massLost, mass, velocity);
			score.addScore(Mathf.FloorToInt(b.Mass));
			b.gameObject.SetActive(false);
		}
		else
		{
			if (mass > 1.5) {
				float massLost = mass * velocity.magnitude;
				mass -= massLost;
				//velocity = elasticity * -velocity;
				velocity = setExitVelocity(b);
				GetComponent<Explosion>().Explode(massLost, mass, velocity);
				pc.BroadcastMessage ("StopOrbit");
				
				float dim = Mathf.Pow (b.gameObject.renderer.bounds.size.x/2f, 0.5f);
				Vector3 bPos = b.transform.position;
				float bodyMass = b.mass;
				
				//GameObject.Destroy(b.gameObject);
				//Smash.generateAsteroids(bPos.x, bPos.y, mass, bodyMass, dim);
			}
		}
		
	}
	
	public void Gravitiate(Body b) {
		float dist = Vector2.Distance (b.gameObject.transform.position, gameObject.transform.position);
		Vector2 r = b.gameObject.transform.position - gameObject.transform.position;
		
		velocity += (G * mass * dist / Mathf.Pow (dist, orbitalStrength) * r);
	}
	
	// Update is called once per frame
	void Update () {
		//check speed
		while (velocity.magnitude > mass*maxSpeed)
			velocity *= .95f;

		//move
		Position += new Vector3(velocity.x, velocity.y, 0);
		
		//correct size
		gameObject.transform.localScale = Vector3.Lerp (gameObject.transform.localScale, maxScale / 20 * mass, Time.deltaTime);
		collider2D.transform.localScale = cMaxScale / 20 * mass;
		
		//update color
		GetComponent<SpriteRenderer>().color = colorOpt.assignColor(mass);
		
	}
	
	void OnMouseDown () {
		pc.BroadcastMessage ("Orbit", this);
	}
	
	void OnMouseUp() {
		pc.BroadcastMessage ("StopOrbit", this);
	}
	
	
	void Stop()
	{
		velocity.x = 0f;
		velocity.y = 0f;
	}
	
	public Vector2 getVelocity()
	{
		return velocity;
	}
	
	public Vector2 setExitVelocity(Body b)
	{
		Vector2 unitNormal = new Vector2(transform.position.x - b.transform.position.x, transform.position.y - b.transform.position.y);
		unitNormal = unitNormal / Mathf.Pow((Mathf.Pow (unitNormal.x, 2) + Mathf.Pow (unitNormal.y, 2)), 0.5f);
		
		Vector2 unitTangent = new Vector2(-unitNormal.y, unitNormal.x);
		
		float intVelNormal = Vector2.Dot(unitNormal, velocity);
		float intVelTangent = Vector2.Dot (unitTangent, velocity);
		
		float finVelTangent = intVelTangent;
		float finVelNormal = (intVelNormal*(mass - b.mass))/(mass + b.mass);
		
		Vector2 finalTangent = finVelTangent*unitTangent;
		Vector2 finalNormal = finVelNormal*unitNormal;
		
		return finalNormal + finalTangent;
	}
	
}
