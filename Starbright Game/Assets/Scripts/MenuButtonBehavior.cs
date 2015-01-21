using UnityEngine;
using System.Collections;

public class MenuButtonBehavior : MonoBehaviour {
	
	public string level;
	private bool loadLevel = false;


	public GameObject targetObject;

	private float time = 0f;

	// Use this for initialization
	void Start () {
	
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
				Game.Instance.Start (level);
			}
		}

		/*
		if(unloadMenu)
		{
			if(time < 0.3f)
			{
				targetObject.GetComponent<Animator>().Play("PanelDown");
				time += Time.deltaTime;
			}
			else
			{
				time = 0f;
				targetObject.GetComponent<Animator>().enabled = false;
				unloadMenu = false;
				//gameObject.GetComponent<PauseButtonBehavior>().ResumeButton();
			}
		}*/

	}

	public void LoadMenu()
	{
		gameObject.GetComponent<Animator>().SetBool("Pressed", true);
		targetObject.GetComponent<Animator>().Play("PanelUp");

	}

	public void LoadLevel()
	{
		gameObject.GetComponent<Animator>().SetBool("Pressed", true);

	}

	public void UnloadMenu()
	{
		targetObject.GetComponent<Animator>().Play("PanelDown");

	}

	public void Restart()
	{
		Debug.Log("RESTART");
		PlayerCharacter.instance.Restart();
	}
}
