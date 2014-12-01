using UnityEngine;
using System.Collections;

public class MultiplierBarBehavior : MonoBehaviour {

	private float initialXScale;

	// Use this for initialization
	void Start () 
	{
		initialXScale = transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.localScale = Scale ();
	}

	float MultiplierPercentage() 
	{
		return ScoreManager.Instance.MultiplierTime / ScoreManager.Instance.MultiplierTimeLimit;
	}

	Vector3 Scale()
	{
		float x = initialXScale * MultiplierPercentage();
		float y = transform.localScale.y;
		float z = transform.localScale.z;
		return new Vector3 (x, y, z);
	}
}
