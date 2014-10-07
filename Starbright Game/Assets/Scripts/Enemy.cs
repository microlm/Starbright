using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private float scale;
	private Vector3 velocity;
	private float speed;
	public Body target; //what it's eating

	// Use this for initialization
	void Start () {
		speed = 0f;
		scale = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		//update speed
		speed = Vector2.Distance (Position, target.Position)*0.001f + 0.01f;

		//update velocity
		Velocity = Angle * Vector3.right * speed;

		//update position
		Position += Velocity;

		//turn towards target
		Quaternion rotation = Quaternion.LookRotation
			(target.Position - Position, transform.TransformDirection(Vector3.up));
		Angle = Quaternion.Slerp(Angle, new Quaternion(0, 0, rotation.z, rotation.w), Time.deltaTime * 10f);

	}

	public Vector3 Position {
		get { return gameObject.transform.position; }
		set {
			//set value and lock z
			float z = gameObject.transform.position.z;
			gameObject.transform.position = new Vector3(value.x, value.y, z);
		}
	}

	public Quaternion Angle {
		get { return gameObject.transform.rotation; }
		set { gameObject.transform.rotation = value; }
	}

	private Vector3 Velocity {
		get { return velocity; }
		set { velocity = value; }
	}
}
