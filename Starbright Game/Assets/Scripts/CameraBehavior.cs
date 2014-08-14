using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {
	
	public GameObject player;
	public float ease = .15f; //camera movement adjustment speed
	private float scalingRate = .05f; //speed of scale
	public float gameScale = 1f; //scale of game to window size, lower number, bigger objects
	public Color backgroundColor = new Color (.05f, .1f, .15f, 1f); //overrides default
	
	private Vector3 scrollingTarget; //when player swipes, new postion that we should follow
	private bool isScrolling; // whether player is swiping to move camera
	
	private float initialMass;
	private float initialSize;
	private float previousSize;
	
	private Vector3 screenCenter;
	
	private Vector3 diff;
	private float minX, minY, maxX, maxY;
	private float mapX=100.0f;
	private float mapY = 100.0f;
	private float border = 0.8f;
	
	private float vertExtent, horzExtent;
	
	Vector3 center;
	private float xratio, yratio;
	
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
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		if (!player.GetComponent<PlayerCharacter>().IsOrbiting()) 
		{
			
			if (isScrolling) {
				
				// if user if scrolling
				// follow scrollig target
				
				transform.position = Vector3.Lerp(transform.position, scrollingTarget, ease * Time.deltaTime);
			}
			else {
				
				// if the player lets go of scrolling,
				// return camera to proper position
				
				if(transform.position != player.transform.position)
				{
					CameraReturn ();
				}
				
				// follow player
				
				transform.position = Vector3.Lerp(transform.position, player.transform.position, ease * Time.deltaTime);
				
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
	
	bool inBounds()
	{
		
		Vector2 pos = player.transform.position;
		
		if((pos.x < maxX && pos.x > minX) && (pos.y < maxY && pos.y > minY))
		{
			return true;
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
	}
	
	// determines how much the camera needs to grow by to keep up with the velocity of the  object
	float projectedGrowthRate()
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
		scrollingTarget = transform.position + scrollAmt;
	}
	
	public void StopScroll() {
		isScrolling = false;
	}
	
}
