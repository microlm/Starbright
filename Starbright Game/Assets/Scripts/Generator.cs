using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Generator : MonoBehaviour {
	
	public static Generator instance;

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
	public AnimationCurve sizeDistribution;
	public AnimationCurve densityByLayer;
	private float genRadius;
	public AnimationCurve blackHoleChance;
	public int maxLayer = 100;

	public bool genBg;

	public float foregroundDepth;
	public float backgroundDepth;
	public ObjectPool foregroundPool;
	public ObjectPool backgroundPool;

	private int[][][] chunks;
	private float xCenter;
	private float yCenter;

	public GameObject foregroundLayer;
	public GameObject backgroundLayer;

	public int bholeChanceOffset;

	private System.Random rand;

	void Start () 
	{	
		if(instance == null)
		{
			instance = this;
		}
		rand = new System.Random();
		chunks = new int[4][][];
		chunks[0] = new int[4][];
		chunks[1] = new int[4][];
		chunks[2] = new int[4][];
		chunks[3] = new int[4][];
		xCenter = 0;
		yCenter = 0;
		genRadius = 2 * areaWidth;
		for(int yShift = 0; yShift < 4; yShift++)
		{
			for(int xShift = 0; xShift < 4; xShift++)
			{
				////////////////////////////////////
				//
				// Where initial generation happens
				//
				////////////////////////////////////
				float xOff = -1 * genRadius + xShift * areaWidth;
				float yOff = -1 * genRadius + yShift * areaHeight;
				chunks[xShift][yShift] = generate(true, xOff, yOff, true);
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		float mult = ProgressCircle.SizeMultiplierFromLayer(ProgressCircle.instance.CurrentLayer);
		////////////////////////////////////
		//
		// There's probably a better way to arrange this, but I'm tired
		//
		////////////////////////////////////
		Vector3 loc = PlayerCharacter.instance.transform.position;
		float xDist = loc.x - xCenter;
		float absXDist = Mathf.Abs(xDist);
		float signX = xDist / absXDist;

		while(absXDist > areaWidth * mult)
		{
			int count = 0; //God have mercy on my soul
			for(float yOff = -1 * genRadius * mult + yCenter; yOff < genRadius * mult + yCenter; yOff += areaHeight * mult)
			{
				if(signX > 0)
				{
					////////////////////////////////////
					//
					// Where further generation happens (POSITIVE X)
					//
					////////////////////////////////////
					destroyChunk(0, count);
					chunks[0][count] = chunks[1][count];
					chunks[1][count] = chunks[2][count];
					chunks[2][count] = chunks[3][count];
					chunks[3][count] = generate(true, genRadius * mult + xCenter, yOff, false);
				}
				else
				{
					////////////////////////////////////
					//
					// Where further generation happens (NEGATIVE X)
					//
					////////////////////////////////////
					destroyChunk(3, count);
					chunks[3][count] = chunks[2][count];
					chunks[2][count] = chunks[1][count];
					chunks[1][count] = chunks[0][count];
					chunks[0][count] = generate(true, -1 * genRadius * mult + xCenter - areaWidth * mult, yOff, false);
			}
				count++;
			}
			xCenter += areaWidth * mult * signX;
			xDist = loc.x - xCenter;
			absXDist = Mathf.Abs(xDist);
		}
		float yDist = loc.y - yCenter;
		float absYDist = Mathf.Abs(yDist);
		float signY = yDist / absYDist;
		while(absYDist > areaHeight * mult)
		{
			int count = 0;
			for(float xOff = -1 * genRadius * mult + xCenter; xOff < genRadius * mult + xCenter; xOff += areaWidth * mult)
			{
				if(signY > 0)
				{
					////////////////////////////////////
					//
					// Where further generation happens (POSITIVE Y)
					//
					////////////////////////////////////
					destroyChunk(count, 0);
					chunks[count][0] = chunks[count][1];
					chunks[count][1] = chunks[count][2];
					chunks[count][2] = chunks[count][3];
					chunks[count][3] = generate(true, xOff, genRadius * mult + yCenter, false);
				}
				else
				{
					////////////////////////////////////
					//
					// Where further generation happens (NEGATIVE Y)
					//
					////////////////////////////////////
					destroyChunk(count, 3);
					chunks[count][3] = chunks[count][2];
					chunks[count][2] = chunks[count][1];
					chunks[count][1] = chunks[count][0];
					chunks[count][0] = generate(true, xOff, -1 * genRadius * mult + yCenter - areaHeight * mult, false);
				}
				count++;
			}
			yCenter += areaWidth * mult * signY;
			yDist = loc.y - yCenter;
			absYDist = Mathf.Abs(yDist);
		}
	}

	public int[] generate(bool isForeground, float xOff, float yOff, bool noHoles)
	{		
		int currentLayer = ProgressCircle.instance.CurrentLayer;
		if (!isForeground)
		{
			currentLayer++;
		}
		float size = ProgressCircle.SizeMultiplierFromLayer(currentLayer);

		float densityMult = densityByLayer.Evaluate ((float) currentLayer / (float)maxLayer);

		float[][] asteroids = ProceduralGeneration.generate(areaWidth * size, areaHeight * size, minDensity * densityMult / size, densityRange * densityMult / size, genChance, minGenSize, genSizeRange, minGenSpacing, genSpacingRange, minAsteroidSize * size, asteroidSizeRange * size, sizeDistribution);
		List<int> ids = new List<int>();
		currentLayer += bholeChanceOffset;
		float curvePosition = ((float)currentLayer / (float)maxLayer);

		float bhChance = blackHoleChance.Evaluate(curvePosition);
		bhChance = bhChance > 0 ? bhChance : 0;
		if(currentLayer <= 1 || noHoles)
			bhChance = 0;

		bool isBlackHole = false;

		foreach(float[] a in asteroids)
		{
			isBlackHole = rand.NextDouble() < bhChance;
			if(isForeground)
			{
				int id = foregroundPool.addBody(a[0] + xOff, a[1] + yOff, foregroundDepth, a[2], false, isBlackHole);
				GameObject asteroid = foregroundPool.getBody(id);
				asteroid.transform.parent = foregroundLayer.transform;
				asteroid.layer = foregroundLayer.layer;
				ids.Add(id);
			}
			else
			{
				int id = backgroundPool.addBody(a[0] + xOff, a[1] + yOff, backgroundDepth, a[2], true, isBlackHole);
				GameObject asteroid = backgroundPool.getBody(id);
				asteroid.transform.parent = backgroundLayer.transform;
				asteroid.layer = backgroundLayer.layer;
				if(!isBlackHole)
					asteroid.rigidbody2D.collider2D.enabled = false;
				ids.Add(id);
			}
		}
		return ids.ToArray();
	}

	public void destroyChunk(int x, int y)
	{
		int[] chunk = chunks[x][y];
		foreach(int id in chunk)
		{
			foregroundPool.removeBody(id);
		}
	}

	public int posHash(int xOff, int yOff)
	{
		return yOff * 512 + xOff;
	}


	public float radiusFromMass(float mass)
	{
		return mass * 0.08f;
	}

	public void LayerUp()
	{ 
		float mult = ProgressCircle.SizeMultiplierFromLayer(ProgressCircle.instance.CurrentLayer);
		foregroundPool.drain();
		for(int yShift = 0 ; yShift < 4; yShift++)
		{
			for(int xShift = 0; xShift < 4; xShift++)
			{
				float xOff = -1 * genRadius + xCenter + (xShift - 1) * areaWidth * mult;
				float yOff = -1 * genRadius + yCenter + (yShift - 1) * areaHeight * mult;
				chunks[xShift][yShift] = generate(true, xOff, yOff, true);
			}
		}
		foregroundPool.removePCOverlap(PlayerCharacter.instance.GetComponent<SpriteRenderer>().bounds);
	}

	public void LayerDown()
	{ 
		float mult = ProgressCircle.SizeMultiplierFromLayer(ProgressCircle.instance.CurrentLayer);
		foregroundPool.drain();
		for(int yShift = 0 ; yShift < 4; yShift++)
		{
			for(int xShift = 0; xShift < 4; xShift++)
			{
				float xOff = -1 * genRadius + xCenter + (xShift - 1) * areaWidth * mult;
				float yOff = -1 * genRadius + yCenter + (yShift - 1) * areaHeight * mult;
				chunks[xShift][yShift] = generate(true, xOff, yOff, true);
			}
		}
		foregroundPool.removePCOverlap(PlayerCharacter.instance.GetComponent<SpriteRenderer>().bounds);
	}
}
