using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {
	
	public float GravityDampening = 1f;
	public float GrowthSpeed = 0.0005f;
	
	void Start () {
	
	}

	void Update () {
		PlayerCharacter.instance.BodyComponent.Gravitiate (this, GravityDampening);
		Scale += Scale * GrowthSpeed;
	}

	void OnCollisionEnter2D(Collision2D c)
	{
		PlayerCharacter.instance.GameOver ();
	}

	public Vector3 Position {
		get { return gameObject.transform.position; }
		set { gameObject.transform.position = value; }
	}

	public Vector3 Scale {
		get { return gameObject.transform.localScale; }
		set { gameObject.transform.localScale = value; }
	}
}
