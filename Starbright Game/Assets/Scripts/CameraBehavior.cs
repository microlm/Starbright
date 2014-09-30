using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {
	
	public GameObject player;
	protected PlayerCharacter pc;
	public float ease = .15f; //camera movement adjustment speed
	protected float scalingRate = .05f; //speed of scale
	public float gameScale = 1f; //scale of game to window size, lower number, bigger objects
	public Color backgroundColor = new Color (.05f, .1f, .15f, 1f); //overrides default
	public bool zoom;
	
	protected Vector3 scrollingTarget; //when player swipes, new postion that we should follow
	protected bool isScrolling; // whether player is swiping to move camera
	
	protected float initialMass;
	protected float initialSize;
	protected float previousSize;
	
	protected Vector3 screenCenter;
	
	protected Vector3 deltaPosition;
	protected Vector3 lastPos;
	protected float minX, minY, maxX, maxY;
	protected float mapX=100.0f;
	protected float mapY = 100.0f;
	protected float border = 0.85f;
	
	protected float vertExtent, horzExtent;

	protected Vector3 center;
	protected float xratio, yratio;

	protected bool outShow;

	protected float cameraDepth;
	
	// Use this for initialization
	void Start () {
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
		lastPos = camera.transform.position;
		cameraDepth = camera.transform.position.z;
		
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		
		moveCamera (1f);
		
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

		deltaPosition = camera.transform.position - lastPos;
		
		lastPos = camera.transform.position;
		
	}

	protected void moveCamera(float speedFactor)
	{
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
				camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 
				                                     initialSize * (currentMass/initialMass) * (1 - (scalingRate * (currentMass-initialMass))), 
				                                     Time.deltaTime * ease);
				
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
		if((pos.x + radius < maxX && pos.x - radius > minX) && (pos.y + radius < maxY && pos.y - radius > minY))
		{
			outShow = true;
			return true;
		}

		if(outShow)
		{
			outShow = false;
		}

		return false;
	}
	
	public void OrthoZoom(Vector3 center)
	{
		xratio = center.x/((maxX-minX)*border);
		yratio = center.y/((maxY-minY)*border);
		Vector2 velocity = player.GetComponent<Body>().getVelocity ();
		
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
		Vector2 vel = player.GetComponent<Body>().getVelocity ();
		
		projectedX = 0;
		projectedY = 0;
		
		if(player.transform.position.x + player.renderer.bounds.size.x/2f > maxX || player.transform.position.x - player.renderer.bounds.size.x/2f < minX)
		{
			projectedX = Mathf.Abs (vel.x);
		}
		
		if(player.transform.position.y + player.renderer.bounds.size.y/2f > maxY || player.transform.position.y - player.renderer.bounds.size.y/2f < minY)
		{
			projectedY = Mathf.Abs (vel.y);
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

	public Vector3 getDeltaPosition()
	{
		return deltaPosition;
	}
	
}
