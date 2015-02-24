using UnityEngine;
using System.Collections;

public class ProgressCircle : MonoBehaviour {

	public static ProgressCircle instance;

	public LevelIndicator levelIndicator;
	public float initialTargetSize;
	public float incrementSize;
	public float initialDecrementSize;
	public Color WarningColor;
	public AnimationCurve WarningColorTransition;
	public float WarningThreshold;

	private float targetSize;
	private float decrementSize;
	private Vector3 maxScale;
	private int currentLayer;
	private Color originalColor;
	private float transisitionTime;

	FlashBehavior flash;
	GameObject backgroundCamera;
	GameObject camera;
	
	void Awake () {
		if (instance == null) {
			instance = this;
		}
		currentLayer = 1;
	}

	// Use this for initialization
	void Start () {
		originalColor = GetComponent<SpriteRenderer> ().color;
		transisitionTime = 0f;
		maxScale = levelIndicator.transform.localScale;
		targetSize = initialTargetSize;
		decrementSize = initialDecrementSize;
		Scale ();

		flash = GameObject.Find ("Flash").GetComponent<FlashBehavior>();
		backgroundCamera = GameObject.Find ("Background Planets Camera");
		camera = GameObject.Find ("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
		if (Game.Instance.State == GameState.Playing)
		{
			PlayerCharacter pc = PlayerCharacter.instance;
			transform.position = pc.transform.position;
			if (Input.GetKeyDown(KeyCode.LeftShift)) {
				PlayerCharacter.instance.Mass = targetSize;

				LevelUp ();


			}
			else if (Input.GetKeyDown(KeyCode.LeftControl)) {
				PlayerCharacter.instance.Mass = decrementSize;

				LevelDown ();
			}

			if (PlayerCharacter.instance.Mass >= targetSize)
			{
				LevelUp ();
			}
			else if (PlayerCharacter.instance.Mass <= decrementSize)
			{
				LevelDown ();
			}

			if (!flash.getWhiteFlash())
			{
				DoneLeveling();
			}

			if (PlayerCharacter.instance.Mass <= GetWarningSize ()) 
			{
				Warning ();
			}
			else 
			{
				ResetColor();
			}
		}
	}

	void LevelUp()
	{
		StartLeveling (true);
		currentLayer++;
		levelIndicator.UpdateLevel(currentLayer, true);
		PlayerCharacter.instance.LevelRefresh ();
		Scale ();
		SoundManager.Instance.PlayMiscSound(1);
		MusicController.Instance.LevelUp ();
		AnalyticsManager.Instance.LogLevelStart(currentLayer);
		AnalyticsManager.Instance.LogAverageFPS();

	}

	void LevelDown()
	{
		if (currentLayer > 1) 
		{
			StartLeveling (false);
			levelIndicator.UpdateLevel(currentLayer, false);
			currentLayer--;
			PlayerCharacter.instance.LevelRefresh ();
			Scale ();
			SoundManager.Instance.PlayMiscSound(2);
			MusicController.Instance.LevelDown ();
			AnalyticsManager.Instance.LogLevelStart(currentLayer);
			AnalyticsManager.Instance.LogAverageFPS();
		}
	}

	void StartLeveling(bool up)
	{
		Time.timeScale = 0f;
		if(up)
		{
			flash.whiteFlash ();
		}
		else
		{
			flash.blackFlash();
		}
	}

	void DoneLeveling()
	{
		Time.timeScale = 1f;
	}

	void Scale()
	{
		targetSize = initialTargetSize * SizeMultiplierFromLayer (currentLayer);
		levelIndicator.transform.localScale = maxScale / 60 * targetSize;
		ColorOption.Instance.maxMass = targetSize * 2.5f;
		ColorOption.Instance.minMass = decrementSize / 1.5f;
	}

	void Warning()
	{
		if (transisitionTime <= 0) {
			transisitionTime = 1f;
		}
		Color color = Color.Lerp (originalColor, WarningColor, WarningColorTransition.Evaluate(transisitionTime));
		GetComponent<SpriteRenderer> ().color = color;

		transisitionTime -= Time.deltaTime;
	}

	void ResetColor()
	{
		GetComponent<SpriteRenderer> ().color = originalColor;
	}

	float GetWarningSize()
	{
		float span = TargetSize - DecrementSize;
		return DecrementSize + span * WarningThreshold;
	}

	public float TargetSize
	{
		get { return targetSize; }
	}

	public float DecrementSize
	{
		get { return decrementSize; }
	}

	public int CurrentLayer
	{
		get { return currentLayer; }
	}

	public static float SizeMultiplierFromLayer(int layer)
	{
		return Mathf.Pow (instance.incrementSize, layer - 1)/Mathf.Pow (instance.incrementSize/1.5f, layer - 1);
	}
}
