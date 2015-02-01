using UnityEngine;
using System.Collections;

public class DustHandler : MonoBehaviour
{

	public float spriteWidth = 16f;
	public float spriteHeight = 10f;
	
	private float xShift;
	private float yShift;
	private float factor;

	// Use this for initialization
	void Start ()
	{
		xShift = 0;
		yShift = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 camPos = BackgroundPlanetsCameraBehavior.instance.camera.transform.position;

		//Modify xShift and yShift to meet camera location (loops mean that if for some reason, we're really far off, it still works)
		for(; camPos.x > (xShift + 0.5f) * spriteWidth; xShift++);
		for(; camPos.x < (xShift - 0.5f) * spriteWidth; xShift--);
		for(; camPos.y > (yShift + 0.5f) * spriteHeight; yShift++);
		for(; camPos.y < (yShift - 0.5f) * spriteHeight; yShift--);

		//Move dust to appropriate location
		transform.position = new Vector3(xShift * spriteWidth, yShift * spriteHeight, transform.position.z);
	}
}
