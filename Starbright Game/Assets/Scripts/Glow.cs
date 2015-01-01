﻿using UnityEngine;
using System.Collections;

public class Glow : MonoBehaviour {
	
	public AnimationCurve transition;

	private Color glowColor;
	private float timer;
	private Color bodyColor;

	// Use this for initialization
	void Start () {
		timer = 0f;
		glowColor = CurrentColor;
		bodyColor = glowColor;
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
			return Color.Lerp (bodyColor, Color.clear, 0.05f);
		}
	}

	public Color Dim
	{
		get
		{
			return Color.Lerp (bodyColor, Color.clear, 0.3f);
		}
	}

	public void setBodyColor(Color c)
	{
		bodyColor = c;
	}

}
