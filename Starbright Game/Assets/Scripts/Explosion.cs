using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public ParticleSystem explosion; 
	
	public void Explode(float mass, float totalMass, Vector2 velocity) {
		GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		expl.particleSystem.startSpeed = velocity.magnitude * 100f;
		expl.particleSystem.Emit ((int)(mass/totalMass * 120f));
		Destroy(expl, 3); // delete the explosion after 3 seconds
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
