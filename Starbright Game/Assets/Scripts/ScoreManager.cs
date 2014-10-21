using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	//gets the current score and updates the gui
	private static int score;
	private static int inc = 5;
	private static int mult;

	// Use this for initialization
	void Start () {
		score = 0;
		mult = 1;
		updateScore ();
	}
	
	// Update is called once per frame
	void Update () {
		updateScore ();
	}

	private void updateScore() {
		guiText.text = Score.ToString();
	}

	public int Score {
		get {
			return score;
		}
	}

	public void addScore() {
		score += inc * mult;
	}

	public int Multiplier {
		get {
			return mult;
		}
		set {
			mult = value;
		}
	}
}
