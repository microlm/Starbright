using UnityEngine;
using System.Collections;

public class ProgressCircle : MonoBehaviour {

	public PlayerCharacter pc;
	public int targetSize;

	private Vector3 maxScale;

	// Use this for initialization
	void Start () {
		maxScale = transform.localScale;

		transform.localScale = maxScale / 20 * targetSize;
	}
	
	// Update is called once per frame
	void Update () {
		//follow player
		transform.position = pc.transform.position;

		//rotate
		transform.Rotate(0, 0, Time.deltaTime * 50f);

		if (pc.Mass >= targetSize) {
			Debug.Log ("YAY");
		}
	}
}
