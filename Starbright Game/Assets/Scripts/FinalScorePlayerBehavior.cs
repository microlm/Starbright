using UnityEngine;
using System.Collections;

public class FinalScorePlayerBehavior : MonoBehaviour {

	float mass;
	public ColorOption colorOpt;
	Vector3 target;

	// Use this for initialization
	void Start () 
	{
		mass = 20f;

		gameObject.transform.localScale = (gameObject.transform.localScale) / 20 * mass;
		GetComponent<SpriteRenderer>().color = PlayerCharacter.instance.BodyComponent.GetComponent<SpriteRenderer>().color;
		Destroy (PlayerCharacter.instance.gameObject);
	}

	void Update () 
	{
	}

	public float getMass()
	{
		return mass;
	}
}
