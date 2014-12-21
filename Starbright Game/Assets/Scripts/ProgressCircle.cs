using UnityEngine;
using System.Collections;

public class ProgressCircle : MonoBehaviour {

	public static ProgressCircle instance;
	public float initialTargetSize;
	public float incrementSize;

	private float targetSize;
	private Vector3 maxScale;
	private int currentLayer;

	void Awake () {
		if (instance == null) {
			instance = this;
		}
		currentLayer = 1;
	}

	// Use this for initialization
	void Start () {
		maxScale = transform.localScale;
		targetSize = initialTargetSize;
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

	public float TargetSize
	{
		get { return targetSize; }
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
