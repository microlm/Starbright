using UnityEngine;
using System.Collections;

public class ProgressCircle : MonoBehaviour {

	public PlayerCharacter pc;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//follow player
		transform.position = pc.transform.position;
		transform.Rotate(0, 0, Time.deltaTime * 50f);
	}
}
