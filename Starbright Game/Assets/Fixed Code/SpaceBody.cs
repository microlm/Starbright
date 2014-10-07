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
		set {
			//set value and lock z
			float z = gameObject.transform.position.z;
			gameObject.transform.position = new Vector3(value.x, value.y, z);
		}
	}

	public float Mass {
		get { return mass; }
		set { mass = value; }
	}

}