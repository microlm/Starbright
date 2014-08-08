using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	int score = 0;

	// Use this for initialization
	void Start () {
		//set text equal to score
		guiText.text = score.ToString(); //GameRunner.Game.Score;
	}
	
	// Update is called once per frame
	void Update () {
		score ++;
		guiText.text = score.ToString();
	}
}
