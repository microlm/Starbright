using UnityEngine;
using System.Collections;

public class MenuButtonBehavior : MonoBehaviour {

	public string level;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClickAction()
	{
		Debug.Log (level);
		Application.LoadLevel(level);
	}
}
