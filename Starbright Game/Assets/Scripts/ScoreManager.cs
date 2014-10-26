using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	//prefab for text that pops up when collecting body
	public GameObject scoreIndicator;
	public GameObject player;

	//gets the current score and updates the gui
	private static int score;

	// Use this for initialization
	void Start () 
	{
		score = 0;
		updateScore ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		updateScore ();
	}

	//updates the GUI
	private void updateScore() 
	{
		guiText.text = Score.ToString();
	}

	public int Score 
	{
		get 
		{
			return score;
		}
	}

	public void addScore(int amt) 
	{
		score += amt;
		popUpScore (amt);
	}

	private void popUpScore(int amt)
	{
		GameObject popUp = Instantiate(scoreIndicator, new Vector3(0f, 0f, -2f), Quaternion.identity) as GameObject;
		popUp.guiText.text = amt.ToString();
		popUp.guiText.pixelOffset = Camera.main.WorldToScreenPoint (player.transform.position);
		Destroy(popUp, 3);
	}
}
