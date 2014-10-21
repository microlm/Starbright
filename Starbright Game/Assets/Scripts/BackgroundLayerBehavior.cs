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
	private float initCameraScale;
	private Vector3 scale;


	// Use this for initialization
	void Start () 
	{
		/*-----------------------------------------------
		 * Initialize the components that are tracked
		 * by the background layer. The pc, main
		 * camera, and  camera scale is tracked. Camera
		 * scale increases/decreases as camera as
		 * camera zooms in/out. The current camera 
		 * scale is tracked so that if there is a 
		 * change, parallax zoom will be implemented.
		 * The factor is the fraction that the pc's size
		 * is of the target size. As the fraction
		 * approaches 1--that is it approaches the target
		 * size--the parallaxing should decrease.
		 * ---------------------------------------------*/
		targetMass = 50;
		pc = player.GetComponent<PlayerCharacter>();
		camera = cam.GetComponent<CameraBehavior>();
		factor = player.GetComponent<Body>().mass / targetMass;

		initCameraScale = cam.orthographicSize;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		/*----------------------------------------------
		 * If the pc is not orbiting a planet, then
		 * the background should move in the opposite
		 * direction to create a parallaxing effect
		 * --------------------------------------------*/
		if(!pc.IsOrbiting())
		{
			factor = player.GetComponent<Body>().mass / targetMass;

			// grabs the distance by which the background layer
			// should be offset to create a parallaxing effect
	
			Vector3 target  = getTarget (1f - factor);
			transform.position = target;
		}

		/*-----------------------------------------------
		 * Within a certain range in which the camera
		 * size changes, the size of the background
		 * planets should increase/decrease and shift
		 * accordingly to lessen the effect of the 
		 * zoom and create a zooming parallax effect.
		 * ---------------------------------------------*/

		if(Mathf.Abs(cam.orthographicSize - initCameraScale) > 0.005f)
		{
			float cameraScale = cam.orthographicSize/initCameraScale;

			float fac = targetMass - player.GetComponent<Body>().mass;
			float scaling = ((cameraScale-1f)*(fac/targetMass)) ;
			scale = new Vector3 (transform.localScale.x + scaling, transform.localScale.y + scaling, transform.localScale.z);
			transform.position = getZoomOffset ((1f-factor) * scale.x/transform.localScale.x); 

			transform.localScale = scale;
			initCameraScale = cam.orthographicSize;

		}
		else if(Mathf.Abs (cam.orthographicSize - initCameraScale) <= 0.005f)
		{
			/*--------------------------------------------------
			 * Because of the small fluctuations in the
			 * main camera's size, there must a be a limit
			 * so that the planet's growth is not too
			 * noticeable. So for extremely small changes,
			 * only the camera scale stored in the background
			 * layer is updated. Making the camera update at
			 * too small a scale will remove the parallax effect
			 * until the player gains enough speed
			 * ------------------------------------------------*/
			initCameraScale = cam.orthographicSize;
		}
	}

	/*-------------------------------------------
	 * Returns the amount that the background
	 * layer should be offset to create a parallax
	 * effect.
	 * -----------------------------------------*/

	Vector3 getTarget(float speedFactor)
	{
		// the offset is determined by how much the
		// camera has moved and how noticeable the
		// the parallax should be; speedfactor is
		// 1f - pc.mass/targetmass and the closer
		// the pc mass is to the target mass, the
		// smaller the offset will be

		Vector3 deltaPos = camera.getDeltaPosition();
		return new Vector3(transform.position.x + (deltaPos.x * speedFactor), transform.position.y + (deltaPos.y * speedFactor), transform.position.z);

	}

	/*-------------------------------------------
	 * The background will expand/contract to
	 * offset the main camera's zoom in/out
	 * -----------------------------------------*/
	Vector3 getZoomOffset(float speedFactor)
	{
		// the speed factor is the offset factor 
		// times the fraction by which the scaling
		// has changed

		Vector3 deltaPos = camera.getDeltaPosition();

		return new Vector3(transform.position.x - (deltaPos.x * speedFactor), transform.position.y - (deltaPos.y * speedFactor), transform.position.z);

	}

	/*------------------------------------------
	 *  Sets the target mass of the current
	 *  layer of planets
	 * ----------------------------------------*/

	public void setTargetMass(float target)
	{
		targetMass = target;
	}

}
