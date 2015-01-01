using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	
	public GameObject explosion1; 
	public GameObject explosion2; 
	public GameObject explosion3; 
	public GameObject explosion4; 
	
	public void Explode(float mass, float totalMass, Vector2 velocity) {
		GameObject exp1 = Instantiate(explosion1, transform.position, Quaternion.identity) as GameObject;
		exp1.particleSystem.startSize = totalMass/10f;
		exp1.particleSystem.startSpeed = velocity.magnitude * 90f;
		exp1.particleSystem.startColor = PlayerCharacter.instance.BodyComponent.BodyColor;
		exp1.particleSystem.Emit ((int)(mass/totalMass * 110f));
		Destroy(exp1, 3); // delete the explosion after 3 seconds

		GameObject exp2 = Instantiate(explosion2, transform.position, Quaternion.identity) as GameObject;
		exp2.particleSystem.startSize = totalMass/12f;
		exp2.particleSystem.startSpeed = velocity.magnitude * 120f;
		exp2.particleSystem.startColor = PlayerCharacter.instance.BodyComponent.BodyColor;
		exp2.particleSystem.Emit ((int)(mass/totalMass * 120f));
		Destroy(exp2, 3);

		GameObject exp3 = Instantiate(explosion3, transform.position, Quaternion.identity) as GameObject;
		exp3.particleSystem.startSize = totalMass/8f;
		exp3.particleSystem.startColor = PlayerCharacter.instance.BodyComponent.BodyColor;
		exp3.particleSystem.startSpeed = velocity.magnitude * 80f;
		exp3.particleSystem.Emit ((int)(mass/totalMass * 140f));
		Destroy(exp3, 3);

		GameObject exp4 = Instantiate(explosion4, transform.position, Quaternion.identity) as GameObject;
		exp4.particleSystem.startSize = totalMass/10f;
		exp4.particleSystem.startColor = PlayerCharacter.instance.BodyComponent.BodyColor;
		exp4.particleSystem.startSpeed = velocity.magnitude * 100f;
		exp4.particleSystem.Emit ((int)(mass/totalMass * 100f));
		Destroy(exp4, 3);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
