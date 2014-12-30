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
	bool disabled = false;

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

		//update if goal size is reached
		if (pc.Mass >= targetSize) {
			LevelUp();
		}
		else if (pc.Mass <= decrementSize) {
			LevelDown();
		}
		else if(disabled)
		{
			disabled = false;
		}

		
		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			PlayerCharacter.instance.Mass = targetSize;
		}
		else if (Input.GetKeyDown(KeyCode.LeftControl)) {
			PlayerCharacter.instance.Mass = decrementSize;
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
		currentLayer++;
		PlayerCharacter.instance.LevelUp ();
		Scale ();
	}

	void LevelDown()
	{
		if (currentLayer > 1) 
		{
			currentLayer--;
			PlayerCharacter.instance.LevelDown ();
			Scale ();
		}
	}

	/*
	// handles moving up a layer
	void LevelUp()
	{
		if(!flash.getWhiteFlash () && !disabled)
		{
			flash.whiteFlash();
			disabled = true;			
		}
		
		
		if(!flash.getWhiteFlash () && disabled)
		{
			foreach(Transform child in pc.gameObject.transform)
			{
				child.gameObject.SetActive(true);
			}

			pc.BodyComponent.enabled = true;
			
			pc.gameObject.GetComponent<CircleCollider2D>().enabled = true;
			
			disabled = false;
			currentLayer++;
			targetSize = initialTargetSize * SizeMultiplierFromLayer(currentLayer);
			Scale ();
		}
		pc.setOrbiting(false);
	}


	public void LevelDown()
	{
		
		if(!flash.getBlackFlash () && !disabled)
		{
			flash.blackFlash();
			Generator.instance.GetComponent<Generator>().LayerDown ();
			
			pc.transform.position = camera.transform.position;
		}
		
		if(flash.getBlackFlash () && !disabled)
		{
			disabled = true;
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
		if (layer <= 1)
			return 1;
		return Mathf.Pow (instance.incrementSize, layer - 1);
	}
}
