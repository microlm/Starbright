using UnityEngine;
using System.Collections;

public class FinalScoreBgCameraBehavior : MonoBehaviour {

	public AnimationCurve pcCameraSpeed;
	float duration = 0f;
	float speed;

	private int borderMinX = -40;
	private int borderMaxX = 45;
	private int borderMinY = - 20;
	private int borderMaxY = 30;

	private Vector3 velocity;
	float vertExtent, horzExtent;
	float minX, maxX, minY, maxY;

	int dirCounter = 0;

	// Use this for initialization
	void Start () 
	{
		float mapX = camera.transform.position.x;
		float mapY = camera.transform.position.y;
		vertExtent = camera.orthographicSize;
		horzExtent = vertExtent * Screen.width / Screen.height;
		
		minX = (mapX - horzExtent);
		maxX = (mapX + horzExtent);
		minY = (mapY - vertExtent);
		maxY = (mapY + vertExtent);

		velocity = new Vector3(1, 1, 0);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void blackHoleControl()
	{
		duration += Time.deltaTime;
		speed = duration/5f;
		camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, camera.orthographicSize * 1.3f, Time.deltaTime);
		Vector3 target = new Vector3(transform.position.x + camera.orthographicSize * 0.33f + pcCameraSpeed.Evaluate(speed) * 1.8f, transform.position.y - camera.orthographicSize * 0.33f, transform.position.z);
		camera.transform.position = Vector3.Lerp (camera.transform.position, target, Time.deltaTime);
	}

	public void pan()
	{
		float mapX = camera.transform.position.x;
		float mapY = camera.transform.position.y;

		minX = (mapX - horzExtent);
		maxX = (mapX + horzExtent);
		minY = (mapY - vertExtent);
		maxY = (mapY + vertExtent);

		if(borderMinX > minX || borderMaxX < maxX)
		{
			velocity.x = -velocity.x;
		}
		if(borderMinY > minY || borderMaxY < maxY)
		{
			velocity.y = -velocity.y;
		}

		if(dirCounter == 10)
		{
			velocity = velocity + pickDirectionChange ();
			dirCounter = 0;
		}

		dirCounter ++;
		camera.transform.position = Vector3.Lerp (camera.transform.position, camera.transform.position + velocity, Time.deltaTime);
	}

	private Vector3 pickDirectionChange()
	{
		int change = Random.Range(0, 3);

		Vector3 dir = new Vector3(0, 0, 0);
		if(change == 0)
		{
			dir = new Vector3(0.05f, 0, 0);
		}
		else if(change == 1)
		{
			dir = new Vector3(-0.05f, 0, 0);
		}
		else if(change == 2)
		{
			dir = new Vector3(0, 0.05f, 0);
		}
		else
		{
			dir = new Vector3(0, -0.05f, 0);
		}
		return dir;
	}

	public void preparePan()
	{
		camera.orthographicSize = 4f;
		float x = Random.Range (borderMinX/1.5f, borderMaxX/1.5f);
		float y = Random.Range (borderMinY/1.5f, borderMaxY/1.5f);
		camera.transform.position = new Vector3(x, y, camera.transform.position.z);
	}
}
