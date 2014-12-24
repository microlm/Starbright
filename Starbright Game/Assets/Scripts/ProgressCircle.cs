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
	}
	
	// Update is called once per frame
	void Update () {
		//follow player
		transform.position = PlayerCharacter.instance.transform.position;

		//update if goal size is reached
		if (PlayerCharacter.instance.Mass >= targetSize) {
			LevelUp();
		}

		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			PlayerCharacter.instance.Mass = targetSize;
			LevelUp ();
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
		targetSize = initialTargetSize * sizeMultiplierFromLayer(currentLayer);
		Scale ();
	}

	void Scale()
	{
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

	public static float sizeMultiplierFromLayer(int layer)
	{
		if (layer <= 1)
			return 1;
		return Mathf.Pow (instance.incrementSize, layer - 1);
	}
}
