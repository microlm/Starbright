using UnityEngine;
using System.Collections;

public class FlashBehavior : MonoBehaviour {

	public AnimationCurve opacity;
	public AnimationCurve size;
	float timer;
	float initSize;

	private bool whiteFlashed = false;
	private bool blackFlashed = false;
	private bool blackOut = false;
	public bool blackFinished = false;

	SpriteRenderer sprite;

	private float screenSize;
	private float scale;

	// Use this for initialization
	void Start () {

		sprite = GetComponent<SpriteRenderer>();
		sprite.color = new Color(1f, 1f, 1f, 0f);
		transform.localScale = new Vector3(0f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (whiteFlashed)
		{
			sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, opacity.Evaluate (timer));
			scale = size.Evaluate (timer) * screenSize;
			sprite.transform.localScale = new Vector3(scale, scale, scale);
			
			timer += Time.deltaTime;

			if(timer >= (opacity[opacity.length - 1].time - 0.01) && timer >= (size[size.length-1].time - 0.01))
			{
				whiteFlashed = false;
			}

		}

		if(blackFlashed)
		{
			sprite.color = new Color (0f, 0f, 0f, opacity.Evaluate (timer));
			scale = size.Evaluate (timer) * screenSize;
			sprite.transform.localScale = new Vector3(scale, scale, scale);
			
			timer += Time.deltaTime;
			
			if(timer >= (opacity[opacity.length - 1].time - 0.01) && timer >= (size[size.length-1].time - 0.01))
			{
				blackFlashed = false;
			}
		}

		if(blackOut)
		{
			sprite.color = new Color (0f, 0f, 0f, opacity.Evaluate (timer));
			timer += Time.deltaTime;
			if(timer >= (opacity[opacity.length - 1].time/2f))
			{
				blackOut = false;
				blackFinished = true;
			}
		}
	}

	public void whiteFlash()
	{
		transform.position = GameObject.Find ("Background Planets Camera").transform.position;
		whiteFlashed = true;
		float screenHeight = Camera.main.orthographicSize * 2f;
		float screenWidth = screenHeight / Screen.height * Screen.width;
		timer = 0f;
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
		timer = 0f;
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
		timer = 0f;
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
	
}
