using UnityEngine;
using System.Collections;

public class ProgressCircle : MonoBehaviour {

	public static ProgressCircle instance;
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
		maxScale = transform.localScale;
		targetSize = initialTargetSize;
		decrementSize = initialDecrementSize;
		Scale ();

		flash = GameObject.Find ("Flash").GetComponent<FlashBehavior>();
		backgroundCamera = GameObject.Find ("Background Planets Camera");
		camera = GameObject.Find ("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
		PlayerCharacter pc = PlayerCharacter.instance;
		transform.position = pc.transform.position;

		Debug.Log (PlayerCharacter.instance.Mass + " " + decrementSize);
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
			Debug.Log ("LEVEL DOWN");
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

	void LevelUp()
	{
		StartLeveling (true);
		currentLayer++;
		PlayerCharacter.instance.LevelUp ();
		Scale ();
	}

	void LevelDown()
	{
		if (currentLayer > 1) 
		{
			StartLeveling (false);
			currentLayer--;
			PlayerCharacter.instance.LevelDown ();
			Scale ();
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

	/*
	// handles moving up a layer
	void LevelUp()
	{
		if(!flash.getWhiteFlash() && !whiteFlashStarted)
		{
			flash.whiteFlash();
			whiteFlashStarted = true;
		}

		if(flash.getWhiteFlash () && flash.getCurrentFrame() == (flash.getMaxFrame() - 10))
		{
			// turns off pc's trail, disables the colliders of the pc, and moves the pc to the location of the bg camera

			foreach(Transform child in pc.gameObject.transform)
			{
				child.gameObject.SetActive(false);
			}

			pc.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			pc.BodyComponent.enabled = false;
			pc.transform.position = backgroundCamera.transform.position;
			camera.transform.position = pc.transform.position;
			Generator.instance.GetComponent<Generator>().LayerUp ();
		}
		
		
		if(!flash.getWhiteFlash () && whiteFlashStarted)
		{
			foreach(Transform child in pc.gameObject.transform)
			{
				child.gameObject.SetActive(true);
			}

			pc.BodyComponent.enabled = true;
			
			pc.gameObject.GetComponent<CircleCollider2D>().enabled = true;
			
			whiteFlashStarted = false;
			currentLayer++;

			targetSize = initialTargetSize * SizeMultiplierFromLayer(currentLayer);

			Scale ();
		}
		pc.setOrbiting(false);
	}


	public void LevelDown()
	{
		
		if(!flash.getBlackFlash () && !blackFlashStarted)
		{
			flash.blackFlash();
			Generator.instance.GetComponent<Generator>().LayerDown ();
			
			pc.transform.position = camera.transform.position;
		}
		
		if(flash.getBlackFlash () && !blackFlashStarted)
		{
			blackFlashStarted = true;
			foreach(Transform child in pc.gameObject.transform)
			{
				child.gameObject.SetActive(false);
			}
			pc.BodyComponent.enabled = false;
		}
		
		if(!flash.getBlackFlash ())
		{
			foreach(Transform child in pc.gameObject.transform)
			{
				child.gameObject.SetActive(true);
			}
			pc.BodyComponent.enabled = true;
			//downMass = downMass/2f;
	
		}
		pc.setOrbiting(false);
	}
	*/

	void Scale()
	{
		targetSize = initialTargetSize * SizeMultiplierFromLayer (currentLayer);
		decrementSize = initialDecrementSize * SizeMultiplierFromLayer (currentLayer);
		transform.localScale = maxScale / 20 * targetSize;
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
		return Mathf.Pow (instance.incrementSize, layer - 1);
	}
}
