using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FinalScoreDirector : MonoBehaviour {

	float time;
	public Camera mainCamera;
	public Camera bgObCamera;
	public GameObject player;
	public GameObject blackhole;
	public GameObject score;
	public GameObject backgroundObs;

	public ParticleSystem exp1;
	public ParticleSystem exp2;
	public ParticleSystem exp3;
	public ParticleSystem exp4;

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

		Debug.Log (ScoreManager.Instance.Score.ToString ());
		GameObject.Find ("FinalScore").GetComponent<Text>().text= ScoreManager.Instance.Score.ToString();
		scoreDisplayed = false;
		score.SetActive(false);

		Destroy(ScoreManager.Instance.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
		Debug.Log((time <= 8f) + " " + (time >= 5f) + " " + ((time <= 8f) && (time >= 5f)));
		if(time <= 5f)
		{
			mainBehavior.PlayerCameraBehavior();
			bgBehavior.blackHoleControl();
		}
		else if((time <= 8f) && (time >= 5f))
		{
			Debug.Log ("Start shakin go");
			mainBehavior.PlayerCameraBehavior();
			bgBehavior.blackHoleControl();
			mainBehavior.Shake(1f);
		}

		else if(time > 8f && !centered && time < (exp1.duration/1.5f + 8f))
		{
			mainCamera.transform.position = player.transform.position;
			centered = true;
			player.SetActive(false);
			blackhole.SetActive (false);
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
		else if(time > 8f && time <= (exp1.duration/1.3f + 8f))
		{
			mainBehavior.SuperNovaZoom();
		}
		else if(time > (exp1.duration/1.3f + 8f) && (time < exp1.duration + 8f) && !scoreDisplayed)
		{
			score.SetActive(true);
			scoreDisplayed = true;
			bgBehavior.preparePan();
			backgroundObs.SetActive(true);

		}
		else if(time > (exp1.duration/1.3f + 8f) && (time < exp1.duration + 8f) && scoreDisplayed)
		{
			mainBehavior.SuperNovaZoom ();
		}
		else if(time > (exp1.duration + 8f))
		{
			bgBehavior.pan();

		}

		time += Time.deltaTime;
	}


}
