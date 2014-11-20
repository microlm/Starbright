using UnityEngine;
using System.Collections;

public class ProgressCircle : MonoBehaviour {

	public PlayerCharacter pc;

	private float targetSize;
	private Vector3 maxScale;

	// Use this for initialization
	void Start () {
		maxScale = transform.localScale;
		targetSize = 7f;

		Scale ();
	}
	
	// Update is called once per frame
	void Update () {
		//follow player
		transform.position = pc.transform.position;

		//rotate
		transform.Rotate(0, 0, Time.deltaTime * 50f);

		//update if goal size is reached
		if (pc.Mass >= targetSize) {
			LevelUp();
		}

		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			pc.Mass = targetSize;
			LevelUp();
			Debug.Log ("Increase level...");
		}
	}

	void LevelUp()
	{
		pc.LevelUp();
		targetSize *= 2.5f;
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
