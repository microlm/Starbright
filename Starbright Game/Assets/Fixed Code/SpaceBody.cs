using UnityEngine;
using System.Collections;

public class SpaceBody : MonoBehaviour {

	/** Mass set from editor */
	public float initialMass;


	private float mass; //body current mass

	// Use this for initialization
	void Start() {

		//set mass
		Mass = initialMass;

		//adjust size to proper mass
	}

	// Update is called once per frame
	void Update() {

	}


	/** GETTERS AND SETTERS */

	public Vector3 Position {
		get { return gameObject.transform.position; }
		set { gameObject.transform.position = value; }
	}

	public float Mass {
		get { return mass; }
		set { mass = value; }
	}

}