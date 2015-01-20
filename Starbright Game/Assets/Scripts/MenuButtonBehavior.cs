using UnityEngine;
using System.Collections;

public class MenuButtonBehavior : MonoBehaviour {
	
	public string level;
	private bool loadLevel = false;
	private bool loadMenu = false;
	private bool unloadMenu = false;
	private bool restart = false;

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

		if(loadMenu)
		{
			if(time < 1f)
			{
				time += Time.deltaTime;
			}
			else
			{
				time = 0f;
				loadMenu = false;
				//gameObject.GetComponent<PauseButtonBehavior>().PauseButton();
			}
		}

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
		}

		if(restart)
		{
			if(time < 1f)
			{
				time += Time.deltaTime;
			}
			else
			{
				time = 0f;
				PlayerCharacter.instance.Restart();
			}
		}

	}

	public void LoadMenu()
	{
		gameObject.GetComponent<Animator>().SetBool("Pressed", true);
		targetObject.GetComponent<Animator>().Play("PanelUp");
		loadMenu = true;
	}

	public void LoadLevel()
	{
		gameObject.GetComponent<Animator>().SetBool("Pressed", true);
		loadLevel = true;
	}

	public void UnloadMenu()
	{
		unloadMenu = true;
	}

	public void Restart()
	{
		restart = true;
		gameObject.GetComponent<Animator>().SetBool("Pressed", true);
	}
}
