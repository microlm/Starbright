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
		target = new Vector3(transform.position.x + (4f/30f) * mass, transform.position.y, -1);
		transform.position = Vector3.Lerp (transform.position, target, Time.deltaTime);
	}

	public float getMass()
	{
		return mass;
	}
}
