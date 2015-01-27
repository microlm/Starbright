using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*-----------------------------------------------
 * This is the class that controls the actions
 * of all the gameObjects during the final score
 * scene's "cutscene".
 * ----------------------------------------------*/

public class FinalScoreDirector : MonoBehaviour {

	float time;
	public Camera mainCamera;
	public Camera bgObCamera;
	public GameObject player;
	public GameObject score;
	public GameObject backgroundObs;

	public ParticleSystem exp1;
	public ParticleSystem exp2;
	public ParticleSystem exp3;
	public ParticleSystem exp4;

	public GameObject hiScore1;
	public GameObject hiScore2;
	public GameObject hiScore3;
	public GameObject hiScore4;
	public GameObject hiScore5;

	private GameObject[] hiScores;

	FinalScoreBgCameraBehavior bgBehavior;
	ScoreCameraBehavior mainBehavior;

	bool centered;
	bool scoreDisplayed;

	// Use this for initialization
	void Start () {
	
		time = 0f;
		bgBehavior = bgObCamera.GetComponent<FinalScoreBgCameraBehavior>();
		mainBehavior = mainCamera.GetComponent<ScoreCameraBehavior>();
		centered = false;

		GameObject.Find ("FinalScore").GetComponent<Text>().text= ScoreManager.Instance.Score.ToString();
		scoreDisplayed = false;
		score.SetActive(false);

		Destroy(ScoreManager.Instance.gameObject);

		//hiScores = new GameObject[5];
		hiScores = new GameObject[5];
		hiScores[0] = hiScore1;
		hiScores[1] = hiScore2; 
		hiScores[2] = hiScore3; 
		hiScores[3] = hiScore4;
		hiScores[4] = hiScore5;
		loadHiScores();
	}
	
	// Calls the actions of each gameObject for the final score cutscene
	void Update () 
	{
		if(time <= 1f)
		{
			// main camera is following the player at varying speeds, player is moving at a constant speed towards the right, and the bgCamera is "zooming out"
			// to show that the blackhole is getting further and further from the player
			mainBehavior.Shake(1f);
		}
		else if(time > 1f && !centered && time < (exp1.duration/1.5f + 1f))
		{
			// explosion of particles begin, player and blackhole assets are both removed, main camera is recentered

			mainCamera.transform.position = player.transform.position;
			centered = true;
			player.SetActive(false);
			exp1.transform.position = mainCamera.transform.position;
			exp2.transform.position = exp1.transform.position;
			exp3.transform.position = exp1.transform.position;
			exp4.transform.position = exp1.transform.position;

			exp1.Play();
			exp2.Play();
			exp3.Play ();
			exp4.Play ();

			mainBehavior.resetDuration();
		}
		else if(time > 1f && time <= (exp1.duration/1.3f + 1f))
		{
			// main camera zooms in and out to make the explosion "pulsate"

			mainBehavior.SuperNovaZoom();
		}
		else if(time > (exp1.duration/1.3f + 1f) && (time < exp1.duration + 1f) && !scoreDisplayed)
		{
			// score is displayed, background camera is prepared for it's panning stage, field of stars is displayed

			score.SetActive(true);
			scoreDisplayed = true;
			bgBehavior.preparePan();
			backgroundObs.SetActive(true);

		}
		else if(time > (exp1.duration/1.3f + 1f) && (time < exp1.duration + 1f) && scoreDisplayed)
		{
			// main camera continues zooming
			mainBehavior.SuperNovaZoom ();
		}
		else if(time > (exp1.duration + 1f))
		{
			// background camera pans across field of stars

			bgBehavior.pan();

		}

		time += Time.deltaTime;
	}
	
	void loadHiScores()
	{
		HighScores hi = new HighScores();
		int added = hi.AddScore(ScoreManager.Instance.Score);

		for(int i=0; i < hiScores.Length; i++)
		{
			if(hi.Scores[i] == 0)
			{
				hiScores[i].GetComponent<Text>().text = (i + 1) + "";
			}
			else
			{
				hiScores[i].GetComponent<Text>().text = (i + 1) + "        " + hi.Scores[i].ToString();
			}
		}

		if(added < 5)
		{
			Color newTextColor = Color.Lerp (Color.yellow, Color.clear, 0.2f);
			hiScores[added].GetComponent<Text>().color = newTextColor;
		}

	}

}
