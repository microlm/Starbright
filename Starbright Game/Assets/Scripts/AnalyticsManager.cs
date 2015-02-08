using UnityEngine;
using System.Collections;

public class AnalyticsManager : MonoBehaviour {

	public GoogleAnalyticsV3 googleAnalytics;
	
	private float totalFPS;
	private long totalFrames;

	// Use this for initialization
	void Start () {
		Instance = this;
		googleAnalytics.StartSession();
	}
	
	// Update is called once per frame
	void Update () {
		CurrentFPS = 1.0f/Time.deltaTime;
		totalFPS += CurrentFPS;
		totalFrames++;
	}

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
		googleAnalytics.DispatchHits();
		totalFPS = 0f;
		totalFrames = 0;
	}

	public static AnalyticsManager Instance 
	{
		get;
		private set;
	}

	public float CurrentFPS
	{
		get;
		private set;
	}

	public float AverageFPS
	{
		get { return totalFPS/totalFrames; }
	}

	public void LogPLayerStart()
	{
		googleAnalytics.LogTiming("Game Start Time", (long)Mathf.FloorToInt(Time.time), "Game Start", "Time player started a game");
	}

	public void LogLevelStart(int level)
	{
		googleAnalytics.LogTiming("Level Start Time", (long)Mathf.FloorToInt(Time.time), "Level " + level, "Level " + level + " start time");
	}

	public void LogPlayerQuit(int level)
	{
		googleAnalytics.LogTiming("Player Quit Time", (long)Mathf.FloorToInt(Time.time), "Quit on level " + level, "Time player quit at level " + level);
	}

	public void LogPlayerDeath()
	{
		googleAnalytics.LogTiming("Player Death Time", (long)Mathf.FloorToInt(Time.time), "Death", "Time player died");
	}

	public void LogPlayerScore(int score)
	{
		googleAnalytics.LogEvent("Score", "Final Score Earned", "Final Player Score Earned", (long)score);
	}

	public void LogAverageFPS()
	{
		googleAnalytics.LogEvent("Performance", "FPS", "Sample Player FPS", (long)Mathf.FloorToInt(AverageFPS));
		totalFPS = 0f;
		totalFrames = 0;
	}
}
