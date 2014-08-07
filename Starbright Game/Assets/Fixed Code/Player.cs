using UnityEngine;
using System.Collections;

public class Player : SpaceBody {

	/** PUBLIC EDITOR VALUES **/
	public Vector2 initialVelocity; //Velocity player starts at
	public float gravity = .0001f; // gravitational constant
	public int orbitalStrength = 2; //power to which we compute gravity


	private Vector2 velocity; //current velocity of movement
	private bool isOrbiting; //whether or not player is orbiting a planet
	private SpaceBody orbitingBody; //what te player is currently orbiting

	// Use this for initialization
	void Start () {
		//set initial values
		Velocity = initialVelocity;

		//not orbiting
		IsOrbiting = false;
		OrbitingBody = null;
	}
	
	// Update is called once per frame
	void Update () {

		//update velocity
		if (IsOrbiting) {
			Gravitate (OrbitingBody);
		}

		//move position by velocity
		Position += new Vector3(Velocity.x, Velocity.y, 0f);
	}


	/** GETTERS AD SETTERS */

	public bool IsOrbiting {
		get { return isOrbiting; }
		set { isOrbiting = value; }
	}

	public SpaceBody OrbitingBody {
		get { return orbitingBody; }
		set { orbitingBody = value; }
	}

	public Vector2 Velocity {
		get { return velocity; }
		set { velocity = value; }
	}


	/** ORBITING FUNCTIONS **/

	//starts player orbiting a specific Body
	public void StartOrbit(SpaceBody b) {
		IsOrbiting = true;
		OrbitingBody = b;
	}

	//stops player from orbiting a planet
	public void StopOrbit() {
		IsOrbiting = false;
	}

	//accelerate according to gravity
	private void Gravitate(SpaceBody b) {
		//compute distance and direction
		float dist = Vector2.Distance (b.gameObject.transform.position, gameObject.transform.position);
		Vector2 r = b.gameObject.transform.position - gameObject.transform.position;

		//update velocity with calculated acceleration
		Velocity += (gravity * Mass * dist / Mathf.Pow (dist, orbitalStrength) * r);
	}
}
