using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {

	public static float elasticity = 0.3f;
	public static float orbitalStrength = 2f;

	private float G = .0001f;
	private float growth = .2f;

	private Vector2 velocity;
	public float mass;
	public Vector2 initialVel;
	private float maxSpeed = .15f;

	private Vector3 maxScale;
	private Vector3 cMaxScale;

	GameObject pc;
	ColorOption colorOpt;
	GameObject camera;

	private static float radius = 1f;

	public float Mass () {
		return mass;
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
	}

	public void Hit(Body b)
	{
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
				GameObject.Destroy(b.gameObject);
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
					//GameObject.Destroy(b.gameObject);
					//collapseAsteroid(mass, dim, bPos);
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
		gameObject.transform.position = new Vector3 (gameObject.transform.position.x + velocity.x,
		                                             gameObject.transform.position.y + velocity.y, 
		                                             gameObject.transform.position.z);
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

	//return current radius of body based on mass and initial width of texture

	public static float radiusFromMass(float mass) {
		return mass/20 * radius;
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

	public void collapseAsteroid(float playerMass, float dim, Vector3 pos)
	{
		GameObject gen = GameObject.Find ("Generator");
		Debug.Log (dim);
		//gen.GetComponent<Generator>().generateAsteroids(dim, dim, pos.x - dim/2f, pos.y - dim/2f, 4f/(dim * dim), 3f, 0.7f, 1, ((int)mass)/((int)playerMass*2), 0, 2, 1f, renderer.bounds.size.x/1.5f, gen.GetComponent<Generator>().sizeDistribution) ;
	}
}
