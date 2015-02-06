using UnityEngine;
using System.Collections;

public class Glow : MonoBehaviour {
	
	public AnimationCurve transition;

	float ActiveGlowOpacity = 1f;
	float InactiveGlowOpacity = 0.5f;


	private Color color;
	private Color glowColor;
	private float timer;

	private bool IsSet;
	public bool IsActive
	{
		get;
		private set;
	}

	// Use this for initialization
	void Start () {
		timer = 1f;
		glowColor = CurrentColor;
		IsSet = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!IsSet)
		{
			if (InactiveColor != null) {
				glowColor = InactiveColor;
				IsSet = true;
			}
		}

		CurrentColor = Color.Lerp (CurrentColor, glowColor, transition.Evaluate(timer));
		timer += Time.deltaTime;
	}

	public bool InTransisition {
		get 
		{
			if ( timer < 1f)
				return true;
			else return false;
		}
	}

	public Color CurrentColor 
	{
		get 
		{
			return gameObject.GetComponent<SpriteRenderer>().color;
		}
		set 
		{
			gameObject.GetComponent<SpriteRenderer>().color = value;
		}
	}

	public Color GlowColor
	{
		get { return glowColor; }
		set 
		{
			glowColor = value;
			timer = 0f;
		}
	}

	public Color DefaultColor
	{
		get { return color; }
		set 
		{ 
			color = value;
			IsSet = false;
		}
	}

	public Color InactiveColor
	{
		get
		{ 
			Color temp = color; 
			temp.a = InactiveGlowOpacity;
			return temp;
		}
	}

	public Color ActiveColor
	{
		get
		{ 
			Color temp = color; 
			temp.a = ActiveGlowOpacity;
			return temp;
		}
	}

	public void Activate()
	{
		IsActive = true;
		GlowColor = ActiveColor;
	}

	public void Deactivate()
	{
		IsActive = false;
		GlowColor = InactiveColor;
	}
}
