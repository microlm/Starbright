using UnityEngine;
using System.Collections;

public class BackgroundCameraBehavior : CameraBehavior{

	public Camera main;
	// Update is called once per frame
	void LateUpdate () 
	{
		moveCamera (0.5f);
		
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
	
}
