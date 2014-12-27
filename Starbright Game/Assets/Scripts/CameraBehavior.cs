using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {
	
	public static CameraBehavior instance;

	public GameObject player;
	protected PlayerCharacter pc;
	public float ease = .15f; //camera movement adjustment speed
	protected float scalingRate = 0.1f; //speed of scale
	public float gameScale = 1f; //scale of game to window size, lower number, bigger objects
	public Color backgroundColor = new Color (.05f, .1f, .15f, 1f); //overrides default
	public bool zoom;

	//shake stuff
	public bool CameraShake; //turn on/off camera shake
	public float MaxShakeTime;	//Max shake time
	public float MaxShakeAmount; //max shake movement
	private float shakeTime; // shakes if shake time >0
	private float shakeAmount;
	
	protected Vector3 scrollingTarget; //when player swipes, new postion that we should follow
	protected bool isScrolling; // whether player is swiping to move camera
	
	protected float initialMass;
	protected float initialSize;
	protected float previousSize;
	
	protected Vector3 screenCenter;

	protected float minX, minY, maxX, maxY;
	protected float mapX=100.0f;
	protected float mapY = 100.0f;
	protected float border = 0.85f;
	
	protected float vertExtent, horzExtent;

	protected Vector3 center;
	protected float xratio, yratio;

	protected float cameraDepth;

	public static CameraBehavior Instance
	{
		get 
		{
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
		instance = this;

		//set bg color
		camera.backgroundColor = backgroundColor;
		
		//no scroll
		isScrolling = false;
		
		//get initial mass
		initialMass = player.GetComponent<Body>().mass;
		
		//set initial size
		camera.orthographic = true;
		camera.orthographicSize *= gameScale;
		initialSize = camera.orthographicSize;
		
		mapX = camera.transform.position.x;
		mapY = camera.transform.position.y;
		vertExtent = camera.orthographicSize/gameScale;
		horzExtent = vertExtent * Screen.width / Screen.height;
		
		minX = (mapX - horzExtent) * border;
		maxX = (mapX + horzExtent) * border;
		minY = (mapY - vertExtent) * border;
		maxY = (mapY + vertExtent) * border;

		pc = player.GetComponent<PlayerCharacter>();
		cameraDepth = camera.transform.position.z;

		shakeTime = 0f;
	}

	// Update is called once per frame
	void LateUpdate () 
	{
		// move the camera
		moveCamera (1f);

		// if shaking
		if (shakeTime > 0)
		{
			ShakeCameraMovement();
			shakeTime -= Time.deltaTime;
		}
		//if shaking has just ended
		else if (shakeTime < 0) 
		{
			CameraReturn ();
			shakeTime = 0f; 
		}

		// if the camera zoom has changed, then adjust border calculations
		if(camera.orthographicSize != vertExtent && !isScrolling)
		{
			vertExtent = camera.orthographicSize/gameScale;
			horzExtent = vertExtent * Screen.width / Screen.height;
			mapX = camera.transform.position.x;
			mapY = camera.transform.position.y;
			
			minX = (mapX - horzExtent) * border;
			maxX = (mapX + horzExtent) * border;
			minY = (mapY - vertExtent) * border;
			maxY = (mapY + vertExtent) * border;


			previousSize = camera.orthographicSize;

		}
	}

	protected void moveCamera(float speedFactor)
	{
		// if camera is not orbiting 
		if (!pc.IsOrbiting()) 
		{
			
			if (isScrolling) 
			{
				
				// if user if scrolling
				// follow scrollig target
				
				transform.position = Vector3.Lerp(transform.position, scrollingTarget, ease * Time.deltaTime);
			}
			else 
			{
				
				// if the player lets go of scrolling,
				// return camera to proper position
				
				if(transform.position != player.transform.position)
				{
					CameraReturn ();
				}
				
				// follow player

				camera.transform.position = Vector3.Lerp(camera.transform.position, getTarget(speedFactor), ease  * Time.deltaTime);
				camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, cameraDepth);

				// scale camera relative to the size of player
				
				previousSize = camera.orthographicSize;
				float currentMass = player.GetComponent<Body>().mass;
				camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, initialSize * (player.GetComponent<Body>().mass/initialMass), Time.deltaTime * ease);
			}
		
		}
		else
		{
			if(!inBounds())
			{
				
				// if off screen set player position as center
				// and zoom in to center
				
				center = player.GetComponent<Body>().transform.position;
				OrthoZoom(center);
			}
		}
	}
	
	protected bool inBounds()
	{
		
		Vector2 pos = player.transform.position;
		float radius = player.renderer.bounds.size.x/2f;
	
		// location relative to camera view port, boundaries at 0 and 1 values
		Vector3 maxPos = camera.WorldToViewportPoint(player.transform.position + player.renderer.bounds.size/2f);
		Vector3 minPos = camera.WorldToViewportPoint(player.transform.position - player.renderer.bounds.size/2f);

		if((maxPos.x < border && minPos.x > 1 - border) && (maxPos.y < border && minPos.y > 1 - border))
		{
			return true;
		}

		return false;
	}
	
	public void OrthoZoom(Vector3 center)
	{
		xratio = center.x/((maxX-minX)*border);
		yratio = center.y/((maxY-minY)*border);
		Vector2 velocity = player.GetComponent<Body>().Velocity;
		
		float growth = projectedGrowthRate();
		//camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, camera.orthographicSize*growth, Time.deltaTime*ease);
		camera.orthographicSize = camera.orthographicSize * growth;
		zoom = true;
	}
	
	// determines how much the camera needs to grow by to keep up with the velocity of the  object
	protected float projectedGrowthRate()
	{
		float projectedX, projectedY, projected, growth;
		float newArea, oldArea;
		Vector2 v = player.GetComponent<Body>().Velocity;
		Vector3 vel = new Vector3(v.x, v.y, 0);
		projectedX = 0;
		projectedY = 0;
		
		Vector3 maxPos = camera.WorldToViewportPoint(player.transform.position + player.renderer.bounds.size/2f);
		Vector3 minPos = camera.WorldToViewportPoint(player.transform.position - player.renderer.bounds.size/2f);
		
		growth = 0;
		
		if(maxPos.x > border || minPos.x < (1 - border))
		{
			if(maxPos.x > border)
			{
				projectedX = (maxPos.x - border) * (maxX - minX);
			}
			else
			{
				projectedX = ((1 - border) - minPos.x) * (maxX - minX);
			}
		}
		
		if(maxPos.y > border || minPos.y < (1 - border))
		{
			if(maxPos.y > border)
			{
				projectedY = (maxPos.y - border) * (maxY - minY);
			}
			else
			{
				projectedY = ((1 - border) - minPos.y) * (maxY - minY);
			}
		}
		
		if(projectedX > projectedY)
		{
			projected = projectedX;
		}
		else
		{
			projected = projectedY;
		}
		
		oldArea = (maxX - minX) * (maxY - minY);
		newArea = ((maxX - minX + projected) * (maxY - minY + projected));
		
		growth = newArea/oldArea;
		
		return growth;
		
	}
	
	public void CameraReturn()
	{
		transform.position = Vector3.Lerp (transform.position, player.transform.position, ease * Time.deltaTime);
		camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, initialSize * (player.GetComponent<Body>().mass/initialMass), Time.deltaTime * ease);
	}
	
	public void Scroll(Vector3 scrollAmt)
	{
		isScrolling = true;
		scrollingTarget = new Vector3(transform.position.x + scrollAmt.x, transform.position.y + scrollAmt.y, transform.position.z);
	}
	
	public void StopScroll() {
		isScrolling = false;
	}

	protected Vector3 getTarget(float speedFactor)
	{
		if(speedFactor == 1f)
		{
			return new Vector3(player.transform.position.x, player.transform.position.y, cameraDepth);
		}
		else
		{
			Vector3 deltaPos = pc.getDeltaPosition();

			return new Vector3(camera.transform.position.x + (deltaPos.x * speedFactor), camera.transform.position.y + (deltaPos.y * speedFactor), cameraDepth);
		}
	}

	/** Shakes the camera if cameraShake is turned on in the unity 
	 * inspector, scaled by percentStrength (default is 100%) */
	public void Shake(float percentStrength = 1f) {
		if (CameraShake) 
		{
			shakeTime = MaxShakeTime;
			shakeAmount = MaxShakeAmount * percentStrength;
		}
	}

	private void ShakeCameraMovement()
	{
		if (shakeAmount > 0) 
		{
			//decrease shake amount over time
			shakeAmount -= MaxShakeAmount / (shakeTime / MaxShakeTime);
		}

		//random position 
		float x = transform.position.x + PositiveOrNegative() * shakeAmount * UnityEngine.Random.Range(0f, 1f);
		float y = transform.position.y + PositiveOrNegative() * shakeAmount * UnityEngine.Random.Range(0f, 1f);

		Vector3 shakePosition = new Vector3 (x, y, transform.position.z);
		camera.transform.position = Vector3.Lerp(transform.position, shakePosition, Time.deltaTime);
		camera.transform.position = new Vector3(transform.position.x, transform.position.y, cameraDepth);
	}

	private int PositiveOrNegative()
	{
		//there is probably a better way to do this but 
		//I just want it dine quick

		int temp = UnityEngine.Random.Range(0, 2);
		if (temp == 0)
			return -1;
		return 1;
	}
}
