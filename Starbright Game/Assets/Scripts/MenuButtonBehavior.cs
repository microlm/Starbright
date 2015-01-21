﻿using UnityEngine;
using System.Collections;

public class MenuButtonBehavior : MonoBehaviour {
	
	public string level;
	private bool loadLevel = false;


	public GameObject targetObject;

	private float time = 0f;

	// Use this for initialization
	void Start () {
	
	}

	void Awake()
	{
		Game.Instance.Resume ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(loadLevel)
		{
			if(time < 1.2f)
			{
				time += Time.deltaTime;
			}
			else
			{
				Application.LoadLevel (level);
			}
		}

	}

	public void LoadMenu()
	{
		gameObject.GetComponent<Animator>().SetBool("Pressed", true);
		targetObject.GetComponent<Animator>().Play("PanelUp");

	}

	public void LoadLevel()
	{
		gameObject.GetComponent<Animator>().SetBool("Pressed", true);
		loadLevel = true;

	}

	public void UnloadMenu()
	{
		targetObject.GetComponent<Animator>().Play("PanelDown");

	}

	public void LoadMainMenu()
	{
		Destroy (PlayerCharacter.instance.gameObject);
		Destroy (targetObject);
		Application.LoadLevel ("Menu");

	}

	public void Quit()
	{
		Application.Quit ();
		Application.runInBackground = false;
	}
}
