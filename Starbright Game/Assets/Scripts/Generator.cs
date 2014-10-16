using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {
	
	public static Generator instance;

	//public ObjectPool pool;

	public GameObject asteroidPrefab;
	public float areaWidth;
	public float areaHeight;
	public float minDensity;
	public float densityRange;
	public double genChance;
	public int minGenSize;
	public int genSizeRange;
	public float minGenSpacing;
	public float genSpacingRange;
	public float minAsteroidSize;
	public float asteroidSizeRange;
	public Gradient sizeDistribution;
	
	// Use this for initialization
	void Start () 
	{	
		if(instance == null)
		{
			instance = this;

			generate(30, 1);
			//generate(40, 3);
		}
	}

	public void generate(float depth, float size)
	{
		float[][] asteroids = ProceduralGeneration.generate(areaWidth, areaHeight, minDensity / size, densityRange / size, genChance, minGenSize, genSizeRange, minGenSpacing,
		                                                    genSpacingRange, minAsteroidSize * size, asteroidSizeRange * size, sizeDistribution);
		
		foreach(float[] a in asteroids)
		{
			{
				GameObject asteroid = (GameObject)Instantiate(asteroidPrefab, new Vector3(a[0] - (areaWidth / 2), a[1] - (areaHeight / 2), depth), Quaternion.identity);
				Body asteroidScript = asteroid.GetComponent<Body>();
				asteroidScript.mass = a[2];
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
