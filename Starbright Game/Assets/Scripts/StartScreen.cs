using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour {

	public float time;

	private float timer;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 30;
		timer = time;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0f )
			Game.Instance.GoToMenu ("Menu");
	}
}
