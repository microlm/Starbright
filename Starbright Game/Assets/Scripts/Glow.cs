using UnityEngine;
using System.Collections;

public class Glow : MonoBehaviour {
	
	public AnimationCurve transition;

	private Color glowColor;
	private float timer;
	private Color brightColor;
	private Color dimColor;

	public bool matching;

	// Use this for initialization
	void Start () {
		timer = 0f;
		glowColor = CurrentColor;
		brightColor = new Color(glowColor.r, glowColor.g, glowColor.b, 1f);
		matching = false;
	}
	
	// Update is called once per frame
	void Update () {
		CurrentColor = Color.Lerp (CurrentColor, glowColor, transition.Evaluate(timer));
		timer += Time.deltaTime;
	}

	private Color CurrentColor 
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
		get 
		{
			return glowColor;
		}
		set 
		{
			glowColor = value;
			timer = 0f;
		}
	}

	public static Color DimWhite 
	{
		get
		{
			return Color.Lerp (Color.white, Color.clear, 0.5f);
		}
	}

	public static Color BrightWhite 
	{
		get
		{
			return Color.Lerp (Color.white, Color.clear, 0.2f);
		}
	}

	public Color Bright
	{
		get
		{
			brightColor = new Color(glowColor.r, glowColor.g, glowColor.b, 1f);
			return Color.Lerp (glowColor, brightColor, 0.2f);
		}
	}

	public Color Dim
	{
		get
		{
			dimColor = new Color(glowColor.r, glowColor.g, glowColor.b, 0.5f);
			return Color.Lerp (glowColor, dimColor, 0.5f);
		}
	}
}
