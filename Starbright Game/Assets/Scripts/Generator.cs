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
	public float genRadius;
	public AnimationCurve blackHoleChance;
	public int maxLayer = 100;

	public bool genBg;

	public float foregroundDepth;
	public float backgroundDepth;
	public ObjectPool foregroundPool;
	public ObjectPool backgroundPool;

	private int[][] keys; //forgive me
	private Dictionary<int, int[]> foregroundChunks;
	private Dictionary<int, int[]> backgroundChunks;
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
		keys = new int[4][];//just hoping this will work well enough to finish this game
		keys[0] = new int[4];
		keys[1] = new int[4];
		keys[2] = new int[4];
		keys[3] = new int[4];
		foregroundChunks = new Dictionary<int, int[]>();
		backgroundChunks = new Dictionary<int, int[]>();
		xCenter = 0;
		yCenter = 0;
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
				keys[xShift][yShift] = posHash(xShift, yShift);
				foregroundChunks.Add(keys[xShift][yShift], generate(true, xOff, yOff));
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
					destroyChunk(keys[0][count], true);
					keys[0][count] = keys[1][count];
					keys[1][count] = keys[2][count];
					keys[2][count] = keys[3][count];
					keys[3][count] = posHash((int)Math.Round(xCenter / areaWidth) + 4, (int)Math.Round(yCenter / areaHeight) + count);
					foregroundChunks.Add(keys[3][count], generate(true, genRadius * mult + xCenter, yOff));
				}
				else
				{
					////////////////////////////////////
					//
					// Where further generation happens (NEGATIVE X)
					//
					////////////////////////////////////
					destroyChunk(keys[3][count], true);
					keys[3][count] = keys[2][count];
					keys[2][count] = keys[1][count];
					keys[1][count] = keys[0][count];
					keys[0][count] = posHash((int)Math.Round(xCenter / areaWidth) - 2, (int)Math.Round(yCenter / areaHeight) - 1 + count);
					foregroundChunks.Add(keys[0][count], generate(true, -1 * genRadius * mult + xCenter - areaWidth * mult, yOff));
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
					destroyChunk(keys[count][0], true);
					keys[count][0] = keys[count][1];
					keys[count][1] = keys[count][2];
					keys[count][2] = keys[count][3];
					keys[count][3] = posHash((int)Math.Round(xCenter / (areaWidth * mult)) + count, (int)Math.Round(yCenter / (areaHeight * mult)) + 4);
					foregroundChunks.Add(keys[count][3], generate(true, xOff, genRadius * mult + yCenter));
				}
				else
				{
					////////////////////////////////////
					//
					// Where further generation happens (NEGATIVE Y)
					//
					////////////////////////////////////
					destroyChunk(keys[count][3], true);
					keys[count][3] = keys[count][2];
					keys[count][2] = keys[count][1];
					keys[count][1] = keys[count][0];
					keys[count][0] = posHash((int)Math.Round(xCenter / (areaWidth * mult)) + count, (int)Math.Round(yCenter / (areaHeight * mult)) - 1);
					foregroundChunks.Add(keys[count][0], generate(true, xOff, -1 * genRadius * mult + yCenter - areaHeight * mult));
				}
				count++;
			}
			yCenter += areaWidth * mult * signY;
			yDist = loc.y - yCenter;
			absYDist = Mathf.Abs(yDist);
		}
	}

	public int[] generate(bool isForeground, float xOff, float yOff)
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
		if(currentLayer <= 1)
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

	public void destroyChunk(int xOff, int yOff, bool isForeground)
	{
		Dictionary<int, int[]> relevantChunks = isForeground ? foregroundChunks : backgroundChunks;
		ObjectPool relevantPool = isForeground ? foregroundPool : backgroundPool;
		int[] chunk = relevantChunks[posHash(xOff, yOff)];
		foreach(int id in chunk)
		{
			relevantPool.removeBody(id);
		}
		relevantChunks.Remove(posHash(xOff, yOff));
	}

	public void destroyChunk(int key, bool isForeground)
	{
		Dictionary<int, int[]> relevantChunks = isForeground ? foregroundChunks : backgroundChunks;
		ObjectPool relevantPool = isForeground ? foregroundPool : backgroundPool;
		int[] chunk = relevantChunks[key];
		foreach(int id in chunk)
		{
			relevantPool.removeBody(id);
		}
		relevantChunks.Remove(key);
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
		foregroundChunks = new Dictionary<int, int[]>();
		for(int yShift = 0 ; yShift < 4; yShift++)
		{
			for(int xShift = 0; xShift < 4; xShift++)
			{
				float xOff = -1 * genRadius + xCenter + xShift * areaWidth * mult;
				float yOff = -1 * genRadius + yCenter + yShift * areaHeight * mult;
				keys[xShift][yShift] =  posHash((int)Math.Round(xOff / (areaWidth * mult)), (int)Math.Round(yOff / (areaHeight * mult)));
				foregroundChunks.Add(keys[xShift][yShift], generate(true, xOff, yOff));
			}
		}
		foregroundPool.removePCOverlap(PlayerCharacter.instance.GetComponent<SpriteRenderer>().bounds);
	}

	public void LayerDown()
	{
		float mult = ProgressCircle.SizeMultiplierFromLayer(ProgressCircle.instance.CurrentLayer);
		foregroundPool.drain();
		foregroundChunks = new Dictionary<int, int[]>();
		for(int yShift = 0 ; yShift < 4; yShift++)
		{
			for(int xShift = 0; xShift < 4; xShift++)
			{
				float xOff = -1 * genRadius + xCenter + xShift * areaWidth * mult;
				float yOff = -1 * genRadius + yCenter + yShift * areaHeight * mult;
				keys[xShift][yShift] =  posHash((int)Math.Round(xOff / (areaWidth * mult)), (int)Math.Round(yOff / (areaHeight * mult)));
				foregroundChunks.Add(keys[xShift][yShift], generate(true, xOff, yOff));
			}
		}
		foregroundPool.removePCOverlap(PlayerCharacter.instance.GetComponent<SpriteRenderer>().bounds);
	}
}
