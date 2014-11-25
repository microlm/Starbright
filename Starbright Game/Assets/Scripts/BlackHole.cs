using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {
	
	public float gravityDampening;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		PlayerCharacter.instance.BodyComponent.Gravitiate (this, gravityDampening);
	}

	public Vector3 Position {
		get { return gameObject.transform.position; }
	}
}
