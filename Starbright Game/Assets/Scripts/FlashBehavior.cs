using UnityEngine;
using System.Collections;

public class FlashBehavior : MonoBehaviour {

	private bool flashed = false;
	private float alpha = 0f;
	private float localScale = 0f;
	SpriteRenderer sprite;
	private bool fade = false;
	private float screenSize;
	// Use this for initialization
	void Start () {

		sprite = GetComponent<SpriteRenderer>();
		sprite.color = new Color(1f, 1f, 1f, 0f);
		transform.localScale = new Vector3(0f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (flashed)
		{
			if(!fade)
			{
				Debug.Log ("localScale1 " + localScale);

				if(alpha < 1f)
				{
					sprite.color = new Color(1f, 1f, 1f, alpha);
					alpha += 0.02f;
				}

				if(localScale <= screenSize)
				{
					localScale += 0.8f;
					transform.localScale = new Vector3(localScale, localScale, localScale);
				}

				if(alpha >= 1f && localScale > screenSize)
				{
					fade = true;
				}
			}

			if(fade)
			{
				Debug.Log ("localScale2 " + localScale + " " + alpha);

				if(alpha >= 0f && localScale > 0f)
				{
					localScale -= screenSize/6f;
					alpha -= 1f/6f;
					transform.localScale = new Vector3(localScale, localScale, localScale);
					sprite.color = new Color(1f, 1f, 1f, alpha);
				}

				else
				{
					transform.localScale = new Vector3(0f, 0f, 0f);
					sprite.color = new Color(1f, 1f, 1f, alpha);
					flashed = false;
					fade = false;
				}
			}
		}
	}

	public void Flash()
	{
		transform.position = GameObject.Find ("Background Planets Camera").transform.position;
		flashed = true;
		float screenHeight = Camera.main.orthographicSize * 2f;
		float screenWidth = screenHeight / Screen.height * Screen.width;

		if(screenHeight > screenWidth)
		{
			screenSize = (screenHeight/(sprite.sprite.bounds.size.y))*2f;
		}
		else
		{
			screenSize = (screenWidth/(sprite.sprite.bounds.size.x))*2f;
		}
	}
	

	public void setFlash(bool f)
	{
		Debug.Log ("SET FLASH");
		flashed = f;
	}

	public bool getFlash()
	{
		return flashed;
	}
}
