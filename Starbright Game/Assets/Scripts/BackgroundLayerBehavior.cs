using UnityEngine;
using System.Collections;

public class BackgroundLayerBehavior : MonoBehaviour {

	public GameObject player;
	public Camera cam;
	private bool zoom;
	private PlayerCharacter pc;
	private CameraBehavior camera;

	public float ease = .15f;
	private float factor;
	private float targetMass;
	private float initCameraScale;
	private Vector3 scale;


	// Use this for initialization
	void Start () 
	{
		pc = player.GetComponent<PlayerCharacter>();
		camera = cam.GetComponent<CameraBehavior>();
		factor = player.GetComponent<Body>().mass / targetMass;

		initCameraScale = cam.orthographicSize;
		Debug.Log (initCameraScale);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!pc.IsOrbiting())
		{
			factor = player.GetComponent<Body>().mass / targetMass;
			transform.position = getTarget (1f-factor);

		}

		if(Mathf.Abs(cam.orthographicSize - initCameraScale) > 0.000002f)
		{
			//Debug.Log (cam.orthographicSize + " " + initCameraScale);
			float cameraScale = cam.orthographicSize/initCameraScale;

			float fac = targetMass - player.GetComponent<Body>().mass;
			float scaling = ((cameraScale-1f)*(fac/targetMass)) ;

			scale = new Vector3 (transform.localScale.x + scaling, transform.localScale.y + scaling, transform.localScale.z);
			transform.position = getZoomOffset ((1f-factor) * scale.x/transform.localScale.x); 
			//Debug.Log (transform.position);
			transform.localScale = scale;
			initCameraScale = cam.orthographicSize;
			zoom = true;
		}
		else if(Mathf.Abs (cam.orthographicSize - initCameraScale) <= 0.000002f)
		{
			initCameraScale = cam.orthographicSize;
		}

		//Debug.Log (transform.localScale);

	}

	Vector3 getTarget(float speedFactor)
	{
		Vector3 deltaPos = camera.getDeltaPosition();
		Debug.Log (camera.getDeltaPosition());
		return new Vector3(transform.position.x + (deltaPos.x * speedFactor), transform.position.y + (deltaPos.y * speedFactor), transform.position.z);

	}

	Vector3 getZoomOffset(float speedFactor)
	{
		Vector3 deltaPos = camera.getDeltaPosition();
		
		return new Vector3(transform.position.x - (deltaPos.x * speedFactor), transform.position.y - (deltaPos.y * speedFactor), transform.position.z);

	}

	public void setTargetMass(float target)
	{
		targetMass = target;
	}

	public void setZoom(bool z)
	{
		zoom = z;
	}
}
