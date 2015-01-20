using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Generator : MonoBehaviour {
	
	public static Generator instance;

	[Tooltip("Base side length, in Unity's units, of a chunk.")]
	public float chunkLen = 40f;

	[Tooltip("Number of seeds created per chunk.")]
	public float density = 2f;
	[Tooltip("How much the density can vary from chunk to chunk, as a percentage. A value of 1 results in a variation from 0 to 2 * density.")]
	public float densityJitter = 0f;

	[Tooltip("Minimum number of children to be generated per iteration on a seed.")]
	public int minimumChildren = 3;
	[Tooltip("Maximum number of children to be generated per iteration on a seed.")]
	public int maximumChildren = 3;

	[Tooltip("Distance between a parent body and it's children, proportionally to the size of the parent. This number will be multiplied by the parents size to generate a value")]
	public float childDistance = 8f;
	[Tooltip("How much the child distance can vary from iteration to iteration, as a percentage. A value of 1 results in a variation from 0 to 2 * child distance.")]
	public float childDistanceJitter = 0f;

	[Tooltip("Size of seed body.")]
	public float initialSize = 40f;
	[Tooltip("How much the initial size can vary from seed to seed, as a percentage. A value of 1 results in a variation from 0 to 2 * initial size.")]
	public float initialSizeJitter = 0f;

	[Tooltip("Size of final generation. The generator will create new children until the children are this percentage of the seed.")]
	public float finalSize = 2.5f;
	[Tooltip("How much the child quantity can vary from seed to seed, as a percentage. A value of 1 results in a variation from 0 to 2 * final size.")]
	public float finalSizeJitter = 0f;

	[Tooltip("The number body size is multiplied by for each iteration of generation.")]
	public float iterationSizeMultiplier = 0.5f;
	[Tooltip("How much the iteration size multiplier can vary from seed to seed, as a percentage. A value of 1 results in a variation from 0 to 2 * iteration size multiplier.")]
	public float iterationSizeMultiplierJitter = 0f;

	[Tooltip("Determines how common various sizes are. Asteroid size is determined by picking a random point along the bottom axis, the multiplying the jitter by the height of the curve - 0.5.")]
	public AnimationCurve sizeDistribution;
	[Tooltip("How density varies from layer to layer. The bottom axis represents what layer we're on, and the left axis is what seed size will be multiplied by.")]
	public AnimationCurve densityByLayer;
	[Tooltip("How black hole chance varies from layer to layer. The bottom axis represents what layer we're on, and the left axis is what seed size will be multiplied by.")]
	public AnimationCurve blackHoleChance;

	[Tooltip("Maximum layer. This also determines the scale of the x axis in the animation curves.")]
	public int maxLayer = 100;

	[Tooltip("Z-value at which the foreground is created.")]
	public float foregroundDepth = 30;

	[Tooltip("Object to hold and manage bodies")]
	public ObjectPool foregroundPool;

	private float genRadius;
	private int[][][] chunks;
	private float xCenter;
	private float yCenter;

	public GameObject foregroundLayer;

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
		genRadius = 2 * chunkLen;
		for(int yShift = 0; yShift < 4; yShift++)
		{
			for(int xShift = 0; xShift < 4; xShift++)
			{
				////////////////////////////////////
				//
				// Where initial generation happens
				//
				////////////////////////////////////
				float xOff = -1 * genRadius + xShift * chunkLen;
				float yOff = -1 * genRadius + yShift * chunkLen;
				chunks[xShift][yShift] = generate(xOff, yOff, true);
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

		while(absXDist > chunkLen * mult)
		{
			int count = 0; //God have mercy on my soul
			for(float yOff = -1 * genRadius * mult + yCenter; yOff < genRadius * mult + yCenter; yOff += chunkLen * mult)
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
					chunks[3][count] = generate(genRadius * mult + xCenter, yOff, false);
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
					chunks[0][count] = generate(-1 * genRadius * mult + xCenter - chunkLen * mult, yOff, false);
			}
				count++;
			}
			xCenter += chunkLen * mult * signX;
			xDist = loc.x - xCenter;
			absXDist = Mathf.Abs(xDist);
		}
		float yDist = loc.y - yCenter;
		float absYDist = Mathf.Abs(yDist);
		float signY = yDist / absYDist;
		while(absYDist > chunkLen * mult)
		{
			int count = 0;
			for(float xOff = -1 * genRadius * mult + xCenter; xOff < genRadius * mult + xCenter; xOff += chunkLen * mult)
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
					chunks[count][3] = generate(xOff, genRadius * mult + yCenter, false);
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
					chunks[count][0] = generate(xOff, -1 * genRadius * mult + yCenter - chunkLen * mult, false);
				}
				count++;
			}
			yCenter += chunkLen * mult * signY;
			yDist = loc.y - yCenter;
			absYDist = Mathf.Abs(yDist);
		}
	}

	public int[] generate(float xOff, float yOff, bool noHoles)
	{		
		int currentLayer = ProgressCircle.instance.CurrentLayer;
		float size = ProgressCircle.SizeMultiplierFromLayer(currentLayer);

		float densityMult = densityByLayer.Evaluate ((float) currentLayer / (float)maxLayer);

		float minDensity = density - densityJitter;
		float densityRange = densityJitter * 2;
		int minGenSize = minimumChildren;
		int genSizeRange = maximumChildren - minimumChildren;
		float minAsteroidSize = initialSize - initialSizeJitter;
		float asteroidSizeRange = initialSizeJitter * 2;
		float minGenSpacing = childDistance - childDistanceJitter;
		float genSpacingRange = childDistanceJitter * 2;

		float[][] asteroids = ProceduralGeneration.generate(chunkLen * size, chunkLen * size, minDensity * densityMult, densityRange * densityMult, minGenSize, genSizeRange, minGenSpacing, genSpacingRange, minAsteroidSize * size, asteroidSizeRange * size, iterationSizeMultiplier, iterationSizeMultiplierJitter, finalSize * size, finalSizeJitter * size, sizeDistribution);
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
			int id = foregroundPool.addBody(a[0] + xOff, a[1] + yOff, foregroundDepth, a[2], false, isBlackHole);
			GameObject asteroid = foregroundPool.getBody(id);
			asteroid.transform.parent = foregroundLayer.transform;
			asteroid.layer = foregroundLayer.layer;
			ids.Add(id);
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


	public static float radiusFromMass(float mass)
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
				float xOff = -1 * genRadius + xCenter + (xShift - 1) * chunkLen * mult;
				float yOff = -1 * genRadius + yCenter + (yShift - 1) * chunkLen * mult;
				chunks[xShift][yShift] = generate(xOff, yOff, true);
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
				float xOff = -1 * genRadius + xCenter + (xShift - 1) * chunkLen * mult;
				float yOff = -1 * genRadius + yCenter + (yShift - 1) * chunkLen * mult;
				chunks[xShift][yShift] = generate(xOff, yOff, true);
			}
		}
		foregroundPool.removePCOverlap(PlayerCharacter.instance.GetComponent<SpriteRenderer>().bounds);
	}
}
