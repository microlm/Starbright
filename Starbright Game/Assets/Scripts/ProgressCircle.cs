using UnityEngine;
using System.Collections;

public class ProgressCircle : MonoBehaviour {

	public float initialTargetSize;
	public float incrementSize;
	public float rotationSpeed;

	private float targetSize;
	private Vector3 maxScale;

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

		//rotate
		transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);

		//update if goal size is reached
		if (PlayerCharacter.instance.Mass >= targetSize) {
			LevelUp();
		}

		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			PlayerCharacter.instance.Mass = targetSize;
			LevelUp();
			Debug.Log ("Increase level...");
		}
	}

	void LevelUp()
	{
		PlayerCharacter.instance.LevelUp();
		targetSize *= incrementSize;
		Scale ();
	}

	public float TargetSize
	{
		get { return targetSize; }
	}

	void Scale()
	{
		transform.localScale = maxScale / 20 * targetSize;
	}
}
