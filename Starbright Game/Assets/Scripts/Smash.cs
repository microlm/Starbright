using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Smash
{

	public Gradient sizeDistribution;
	public GameObject asteroidPrefab;
	static System.Random rand = new System.Random();
	// Use this for initialization

	public static void generateAsteroids(float x, float y, float playerMass, float bodyMass, float dim)
	{
		Debug.Log (dim);
		/*float size = playerMass/2f + (1 - sizeDistribution.Evaluate((float)rand.NextDouble()).grayscale) * (bodyMass/playerMass);
		List<float[]> asteroids = ProceduralGeneration.growSeed(x, y, size, 0.5, 2, (int)(dim/(2*size) + 2), dim/4f, dim/2f, size, bodyMass-size, sizeDistribution);
		Debug.Log ("howdy");
		foreach(float[] a in asteroids)
		{
			GameObject asteroid = (GameObject)Instantiate(asteroidPrefab, new Vector3(a[0] - (dim / 2), a[1] - (dim / 2), 30), Quaternion.identity);
			Body asteroidScript = asteroid.GetComponent<Body>();
			asteroidScript.mass = a[2];

		}*/
	}

}
