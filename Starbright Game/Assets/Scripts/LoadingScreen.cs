using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour {

	public string level;

	// Use this for initialization
	void Start () {
		Application.LoadLevel (level);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
