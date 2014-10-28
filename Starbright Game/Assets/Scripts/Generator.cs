using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public float genRadius;

	public float foregroundDepth;
	public float backgroundDepth;
	public ObjectPool foregroundPool;
	public ObjectPool backgroundPool;
	
	private Dictionary<float, int[]> foregroundChunks;
	private Dictionary<float, int[]> backgroundChunks;
	private float xCenter;
	private float yCenter;
	
	public GameObject foregroundLayer;
	public GameObject backgroundLayer;

	void Start () 
	{	
		if(instance == null)
		{
			instance = this;
		}

		foregroundChunks = new Dictionary<float, int[]>();
		backgroundChunks = new Dictionary<float, int[]>();
		xCenter = 0;
		yCenter = 0;
		for(float yOff = -1 * genRadius; yOff < genRadius; yOff += areaHeight)
		{
			for(float xOff = -1 * genRadius; xOff < genRadius; xOff += areaWidth)
			{
				////////////////////////////////////
				//
				// Where initial generation happens
				//
				////////////////////////////////////
				foregroundChunks.Add(posHash(xOff, yOff), generate(true, 1, xOff, yOff));
				backgroundChunks.Add(posHash(xOff, yOff), generate(false, 2, xOff, yOff));		
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		////////////////////////////////////
		//
		// There's probably a better way to arrange this, but I'm tired
		//
		////////////////////////////////////
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
					////////////////////////////////////
					//
					// Where further generation happens (POSITIVE X)
					//
					////////////////////////////////////
					destroyChunk(-1 * genRadius + xCenter, yOff, true);
					foregroundChunks.Add(posHash(genRadius + xCenter, yOff), generate(true, 1, genRadius + xCenter, yOff));
					destroyChunk(-1 * genRadius + xCenter, yOff, false);
					backgroundChunks.Add(posHash(genRadius + xCenter, yOff), generate(false, 2, genRadius + xCenter, yOff));
				}
				else
				{
					////////////////////////////////////
					//
					// Where further generation happens (NEGATIVE X)
					//
					////////////////////////////////////
					destroyChunk(genRadius + xCenter - areaWidth, yOff, true);
					foregroundChunks.Add(posHash(-1 * genRadius + xCenter - areaWidth, yOff), generate(true, 1, -1 * genRadius + xCenter - areaWidth, yOff));
					destroyChunk(genRadius + xCenter - areaWidth, yOff, false);
					backgroundChunks.Add(posHash(-1 * genRadius + xCenter - areaWidth, yOff), generate(false, 2, -1 * genRadius + xCenter - areaWidth, yOff));
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
					////////////////////////////////////
					//
					// Where further generation happens (POSITIVE Y)
					//
					////////////////////////////////////
					destroyChunk(xOff, -1 * genRadius + yCenter, true);
					foregroundChunks.Add(posHash(xOff, genRadius + yCenter), generate(true, 1, xOff, genRadius + yCenter));
					destroyChunk(xOff, -1 * genRadius + yCenter, false);
					backgroundChunks.Add(posHash(xOff, genRadius + yCenter), generate(false, 2, xOff, genRadius + yCenter));
				}
				else
				{
					////////////////////////////////////
					//
					// Where further generation happens (NEGATIVE Y)
					//
					////////////////////////////////////
					destroyChunk(xOff, genRadius + yCenter - areaHeight, true);
					foregroundChunks.Add(posHash(xOff, -1 * genRadius + yCenter - areaHeight), generate(true, 1, xOff, -1 * genRadius + yCenter - areaHeight));
					destroyChunk(xOff, genRadius + yCenter - areaHeight, false);
					backgroundChunks.Add(posHash(xOff, -1 * genRadius + yCenter - areaHeight), generate(false, 2, xOff, -1 * genRadius + yCenter - areaHeight));
				}
			}

			yCenter += areaWidth * signY;
			yDist = loc.y - yCenter;
			absYDist = Mathf.Abs(yDist);
		}
	}

	public int[] generate(bool isForeground, float size, float xOff, float yOff)
	{		
		float[][] asteroids = ProceduralGeneration.generate(areaWidth, areaHeight, minDensity / size, densityRange / size, genChance, minGenSize, genSizeRange, minGenSpacing,
		                                                    genSpacingRange, minAsteroidSize * size, asteroidSizeRange * size, sizeDistribution);
		List<int> ids = new List<int>();
		
		foreach(float[] a in asteroids)
		{
			if(isForeground)
			{
				int id = foregroundPool.addBody(a[0] + xOff, a[1] + yOff, foregroundDepth, a[2]);
				GameObject asteroid = foregroundPool.getBody(id);
				asteroid.transform.parent = foregroundLayer.transform;
				ids.Add(id);
			}
			else
			{
				int id = backgroundPool.addBody(a[0] + xOff, a[1] + yOff, backgroundDepth, a[2]);
				GameObject asteroid = backgroundPool.getBody(id);
				asteroid.transform.parent = backgroundLayer.transform;
				asteroid.rigidbody2D.collider2D.enabled = false;
				ids.Add(id);
			}
		}
		return ids.ToArray();
	}

	public void destroyChunk(float xOff, float yOff, bool isForeground)
	{
		Dictionary<float, int[]> relevantChunks = isForeground ? foregroundChunks : backgroundChunks;
		ObjectPool relevantPool = isForeground ? foregroundPool : backgroundPool;
		int[] chunk = relevantChunks[posHash(xOff, yOff)];
		foreach(int id in chunk)
		{
			relevantPool.removeBody(id);
		}
		relevantChunks.Remove(posHash(xOff, yOff));
	}
	
	public float posHash(float xOff, float yOff)
	{
		return yOff * genRadius * 512f + xOff;
	}
}
