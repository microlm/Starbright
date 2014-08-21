using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	//gets the current score and updates the gui


	// Use this for initialization
	void Start () {
		updateScore ();
	}
	
	// Update is called once per frame
	void Update () {
		updateScore ();
	}

	private void updateScore() {
		guiText.text = GameRunner.Game.Score.Value.ToString();
	}
	
}
