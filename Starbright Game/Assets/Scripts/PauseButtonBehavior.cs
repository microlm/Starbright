using UnityEngine;
using System.Collections;

public class PauseButtonBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PauseButton()
	{
		Game.Instance.Pause ();
	}

	public void ResumeButton()
	{
		Game.Instance.Resume ();
	}
}
