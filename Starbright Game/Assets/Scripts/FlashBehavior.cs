using UnityEngine;
using System.Collections;

public class FlashBehavior : MonoBehaviour {

	public AnimationCurve opacity;
	public AnimationCurve size;

	float initSize;

	private bool whiteFlashed = false;
	private bool blackFlashed = false;
	private bool blackOut = false;
	public bool blackFinished = false;

	SpriteRenderer sprite;

	private float screenSize;
	private float scale;

	private float maxFrame = 60;
	private float currentFrame = 0;

	// Use this for initialization
	void Start () {

		sprite = GetComponent<SpriteRenderer>();
		sprite.color = new Color(1f, 1f, 1f, 0f);
		transform.localScale = new Vector3(0f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (whiteFlashed)
		{
			sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, opacity.Evaluate ((currentFrame/maxFrame))*opacity[opacity.length - 1].time);
			scale = size.Evaluate ((currentFrame/maxFrame) * size[size.length - 1].time) * screenSize;
			sprite.transform.localScale = new Vector3(scale, scale, scale);
			currentFrame++;

			if(currentFrame > maxFrame)
			{
				whiteFlashed = false;
				currentFrame = 0;
			}


		}

		if(blackFlashed)
		{
			sprite.color = new Color (0f, 0f, 0f, opacity.Evaluate ((currentFrame/maxFrame)*opacity[opacity.length - 1].time));
			scale = size.Evaluate ((currentFrame/maxFrame)*size[size.length - 1].time) * screenSize;
			sprite.transform.localScale = new Vector3(scale, scale, scale);
			currentFrame ++;

			if(currentFrame > maxFrame)
			{
				blackFlashed = false;
				currentFrame = 0;
			}

		}

		if(blackOut)
		{
			sprite.color = new Color (0f, 0f, 0f, opacity.Evaluate ((currentFrame/maxFrame)*opacity[opacity.length - 1].time));
			currentFrame ++;

			if(currentFrame == (maxFrame / 2f))
			{
				blackOut = false;
				blackFinished = true;
				currentFrame = 0;
			}

		}
	}

	public void whiteFlash()
	{
		transform.position = GameObject.Find ("Main Camera").transform.position;
		whiteFlashed = true;
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

	public void blackFlash()
	{
		transform.position = GameObject.Find ("Main Camera").transform.position;
		blackFlashed = true;
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

	public void blackScreen()
	{
		transform.position = GameObject.Find ("Main Camera").transform.position;
		blackOut = true;
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

		sprite.transform.localScale = new Vector3(screenSize, screenSize, 0f);
	}

	public void setWhiteFlash(bool f)
	{
		whiteFlashed = f;
	}

	public bool getWhiteFlash()
	{
		return whiteFlashed;
	}

	public void setBlackFlash(bool f)
	{
		blackFlashed = true;
	}

	public bool getBlackFlash()
	{
		return blackFlashed;
	}

	public float getCurrentFrame()
	{
		return currentFrame;
	}

	public float getMaxFrame()
	{
		return maxFrame;
	}

	public bool isMaxFlash()
	{
		return (getWhiteFlash () && getCurrentFrame () == (getMaxFrame () - 10));
	}
	
}
