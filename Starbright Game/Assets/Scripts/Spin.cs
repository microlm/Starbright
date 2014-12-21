using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {

	public float rotationSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//rotate
		transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
	}
}
