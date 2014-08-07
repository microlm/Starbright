using UnityEngine;
using System.Collections;

public class Star : SpaceBody {

	public float breakingForce;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//player collides with star
	void Collide (Player p) {
		//check if player has enoghforce to light up star
		if ( (p.Velocity.magnitude * p.Mass) >= breakingForce) {
			LightUp ();
		}
		//else react
		else Bounce (breakingForce - p.Velocity.magnitude * p.Mass);

	}

	void LightUp () {

	}

	void Bounce (float force){}
}
