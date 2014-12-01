using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	//prefab for text that pops up when collecting body
	public GameObject scoreIndicator;
	public float multiplierTimeLimit;
	public float multiplierCap;

	//gets the current score and updates the gui
	private static int score;
	private float time;
	private int multiplier;
	private static ScoreManager instance;

	// Use this for initialization
	void Start () 
	{
		instance = this;
		score = 0;
		time = 0f;
		multiplier = 1;
		updateScore ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		updateScore ();
		//if there is time left for the multiplier
		if ( time > 0 ) {
			// progress timer
			time -= Time.deltaTime;
		} else {
			//otherwise reset
			multiplier = 1;
		}
	}

	//updates the GUI
	private void updateScore() 
	{
		guiText.text = Score.ToString();
	}

	public static ScoreManager Instance
	{
		get 
		{
			return instance;
		}
	}

	public int Score 
	{
		get 
		{
			return score;
		}
	}

	public int Multiplier 
	{
		get
		{
			return multiplier;
		}
	}

	/** Time remaining for multiplier to be active */
	public float MultiplierTime
	{
		get
		{
			return time;
		}
	}

	/** Total time that the multiplier will be active */
	public float MultiplierTimeLimit
	{
		get 
		{
			return multiplierTimeLimit; 
		}
	}

	/** Removes multiplier bonus */
	public void ResetMultiplier()
	{
		multiplier = 1;
	}

	public void AddScore(int amt) 
	{
		score += amt * multiplier;
		popUpScore (amt, multiplier);
		if (multiplier < multiplierCap)
			multiplier += 1;
		time = multiplierTimeLimit;
	}

	private void popUpScore(int amt, int multiplier)
	{
		GameObject popUp = Instantiate(scoreIndicator, new Vector3(0f, 0f, -2f), Quaternion.identity) as GameObject;
		string scoreText = amt.ToString ();
		if (multiplier > 1)
			scoreText += " x " + multiplier.ToString();
		popUp.guiText.text = scoreText;
		popUp.guiText.pixelOffset = Camera.main.WorldToScreenPoint (PlayerCharacter.instance.transform.position);
		Destroy(popUp, 3);
	}
}
