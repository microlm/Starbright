using UnityEngine;
using System.Collections;

public class PanelFadeOut : MonoBehaviour {

	bool isActive;
	Animator animator;
	float time;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		time = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isActive && time >= 1f)
		{
			isActive = false;
			time = 0f;
		}

		if(isActive)
		{
			time += Time.deltaTime;
		}

	}

	public void fadeOut()
	{
		animator.Play("HighScorePanelFadeOut");
	
		isActive = true;
	}

	public void fadeIn()
	{
		animator.enabled = true;
		animator.Play ("HighScorePanelFadeIn");
		isActive = true;
	}
}
