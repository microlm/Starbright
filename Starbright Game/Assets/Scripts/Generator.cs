using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public float genRadius;

	private ObjectPool pool;
	private Dictionary<float, int[]> chunks;
	private float xCenter;
	private float yCenter;
	
	// Use this for initialization
	void Start () 
	{	
		if(instance == null)
		{
			instance = this;
		}
		chunks = new Dictionary<float, int[]>();
		xCenter = 0;
		yCenter = 0;
		for(float yOff = -1 * genRadius; yOff < genRadius; yOff += areaHeight)
		{
			for(float xOff = -1 * genRadius; xOff < genRadius; xOff += areaWidth)
			{
				Debug.Log(xOff + ", " + yOff);
				chunks.Add(posHash(xOff, yOff), generate(30, 1, xOff, yOff));
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		Vector3 loc = PlayerCharacter.instance.transform.position;
		float xDist = loc.x - xCenter;
		float absXDist = Mathf.Abs(xDist);
		float signX = xDist / absXDist;
		while(absXDist > areaWidth)
		{
			for(float yOff = -1 * genRadius + yCenter; yOff < genRadius + yCenter; yOff += areaHeight)
			{
				if(signX > 0)
				{
					destroyChunk(-1 * genRadius + xCenter, yOff);
					generate(30, 1, genRadius + xCenter, yOff);
				}
				else
				{
					destroyChunk(genRadius + xCenter - areaWidth, yOff);
					generate(30, 1, -1 * genRadius + xCenter - areaWidth, yOff);
				}
			}
			xCenter += areaWidth * signX;
			xDist = loc.x - xCenter;
			absXDist = Mathf.Abs(xDist);
		}
		float yDist = loc.y - yCenter;
		float absYDist = Mathf.Abs(yDist);
		float signY = yDist / absYDist;
		while(absYDist > areaHeight)
		{
			for(float xOff = -1 * genRadius + xCenter; xOff < genRadius + xCenter; xOff += areaWidth)
			{
				if(signY > 0)
				{
					destroyChunk(xOff, -1 * genRadius + yCenter);
					generate(30, 1, xOff, genRadius + yCenter);
				}
				else
				{
					destroyChunk(xOff, genRadius + yCenter - areaHeight);
					generate(30, 1, xOff, -1 * genRadius + yCenter - areaHeight);
				}
			}
			yCenter += areaWidth * signY;
			yDist = loc.y - yCenter;
			absYDist = Mathf.Abs(yDist);
		}
	}

	public int[] generate(float depth, float size, float xOff, float yOff)
	{
		if(pool == null)
		{
			pool = GetComponent<ObjectPool>();
		}
		
		float[][] asteroids = ProceduralGeneration.generate(areaWidth, areaHeight, minDensity / size, densityRange / size, genChance, minGenSize, genSizeRange, minGenSpacing,
		                                                    genSpacingRange, minAsteroidSize * size, asteroidSizeRange * size, sizeDistribution);
		List<int> ids = new List<int>();
		
		foreach(float[] a in asteroids)
		{
			{
				ids.Add(pool.addBody(a[0] + xOff, a[1] + yOff, depth, a[2]));
			}
		}
		return ids.ToArray();
	}

	public void destroyChunk(float xOff, float yOff)
	{
		int[] chunk = chunks[posHash(xOff, yOff)];
		foreach(int id in chunk)
		{
			pool.removeBody(id);
		}
		chunks.Remove(posHash(xOff, yOff));
	}
	
	public float posHash(float xOff, float yOff)
	{
		return yOff * genRadius * 512f + xOff;
	}
}
