using UnityEngine;
using System.Collections;

public class BackgroundLayerBehavior : MonoBehaviour {

	public GameObject player;
	public Camera cam;
	private PlayerCharacter pc;
	private CameraBehavior camera;

	public float ease = .15f;
	private float factor;
	private float targetMass;

	// Use this for initialization
	void Start () 
	{
		pc = player.GetComponent<PlayerCharacter>();
		camera = cam.GetComponent<CameraBehavior>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!pc.IsOrbiting())
		{
			Debug.Log (player.GetComponent<Body>().mass);
			factor = player.GetComponent<Body>().mass / targetMass;
			transform.position = getTarget (factor);
		}
	;
	}

	Vector3 getTarget(float speedFactor)
	{
		Vector3 deltaPos = camera.getDeltaPosition();
	
		return new Vector3(transform.position.x + (deltaPos.x * speedFactor), transform.position.y + (deltaPos.y * speedFactor), transform.position.z);

	}

	public void setTargetMass(float target)
	{
		targetMass = target;
	}
}
