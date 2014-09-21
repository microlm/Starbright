using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {
	
	public static Generator instance;

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

	private GameObject backgroundLayer;
	private GameObject foregroundLayer;

	// Use this for initialization
	void Start () 
	{	
		backgroundLayer = GameObject.Find ("Background Planets");
		foregroundLayer = GameObject.Find ("Foreground Planets");

		if(instance == null)
		{
			instance = this;
		}

		generate(30, 1f, 0.3f);
		generate(40, 20f, 0.1f);

		backgroundLayer.GetComponent<BackgroundLayerBehavior>().setTargetMass(20f);

	}

	public void generate(float depth, float size, float density)
	{
		float[][] asteroids = ProceduralGeneration.generate(areaWidth, areaHeight, minDensity / size, densityRange / size, genChance, minGenSize, genSizeRange, minGenSpacing,
		                                                    genSpacingRange, minAsteroidSize * size, asteroidSizeRange * size, sizeDistribution);
		
		foreach(float[] a in asteroids)
		{
			{
				GameObject asteroid = (GameObject)Instantiate(asteroidPrefab, new Vector3(a[0] - (areaWidth / 2), a[1] - (areaHeight / 2), depth), Quaternion.identity);
				Body asteroidScript = asteroid.GetComponent<Body>();
				asteroidScript.mass = a[2];

				if(depth > 30)
				{
					asteroid.transform.parent = backgroundLayer.transform;
					asteroid.rigidbody2D.collider2D.enabled = false;
				}
				else
				{
					asteroid.transform.parent = foregroundLayer.transform;

				}

			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
