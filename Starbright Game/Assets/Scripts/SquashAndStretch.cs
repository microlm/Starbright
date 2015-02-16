using UnityEngine;
using System.Collections;

public class SquashAndStretch : MonoBehaviour {

	public float squashFactor;
	public float stretchFactor;

	private Body body;
	private float stretch;

	// Use this for initialization
	void Start () {
		body = GetComponent<Body>();
		stretch = 1;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Rotate();
		Stretch();
	}

	/** Rotate depending on direction of velocity */
	void Rotate()
	{
		Vector2 dir = body.Velocity;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		body.Rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	/** Strech out when going faster */	
	void Stretch()
	{
		float lastStretch = stretch;
		stretch = body.Velocity.magnitude * stretchFactor;
		Vector3 scale = body.Scale;
		scale.x *= 1/lastStretch;
		scale.x *= stretch;
		body.Scale = scale;
		Debug.Log (stretch);
	}
}
