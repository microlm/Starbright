using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {


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

	public bool[] spaceOccupation;
	public int occupationPrecision = 20;

	// Use this for initialization
	void Start () 
	{	
		float[][] asteroids = ProceduralGeneration.generate(areaWidth, areaHeight, minDensity, densityRange, genChance, minGenSize, genSizeRange, minGenSpacing,
		                                                    genSpacingRange, minAsteroidSize, asteroidSizeRange, sizeDistribution);

		foreach(float[] a in asteroids)
		{
			//if(!checkOccupied(a))
			{
				GameObject asteroid = (GameObject)Instantiate(asteroidPrefab, new Vector3(a[0] - (areaWidth / 2), a[1] - (areaHeight / 2), 30), Quaternion.identity);
				Body asteroidScript = asteroid.GetComponent<Body>();
				asteroidScript.mass = a[2];
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool checkOccupied(float[] a)
	{
		if(spaceOccupation.Length == 0)
		{
			spaceOccupation = new bool[(int)areaWidth * (int)areaHeight * square(occupationPrecision)];
		}

		bool occupied = false;

		for (int i = (int)a[0]; i < (int)(Body.radiusFromMass(a[2]) * occupationPrecision) + 1; i++) 
		{
			for(int j = 0; square(j) < (int)square(Body.radiusFromMass(a[2]) * occupationPrecision) + square(i); j++)
			{
				if(spaceOccupation[indexFromCoords(i, (int)a[1] + j)] || spaceOccupation[indexFromCoords(i, (int)a[1] - j)])
				{
					occupied = true;
					break;
				}
			}
			if(occupied)
			{
				Debug.Log("Found some overlap!");
				break;
			}
		}

		if (occupied) 
		{
			return true;
		}

		for (int i = (int)a[0]; i < (int)(Body.radiusFromMass(a[2]) * occupationPrecision) + 1; i++) 
		{
			for(int j = 0; square(j) < (int)square(Body.radiusFromMass(a[2]) * occupationPrecision) + square(i); j++)
			{
				spaceOccupation[indexFromCoords(i, (int)a[1] + j)] = true;
				spaceOccupation[indexFromCoords(i, (int)a[1] - j)] = true;
			}
		}

		return false;
	}

	public int indexFromCoords(int x, int y)
	{
		return x + (y * (int)areaWidth * occupationPrecision);
	}

	public int square(int i)
	{
		return i * i;
	}
	
	public double square(double d)
	{
		return d * d;
	}
}
