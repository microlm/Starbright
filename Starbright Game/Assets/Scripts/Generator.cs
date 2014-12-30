using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public float genRadius;
	public AnimationCurve blackHoleChance;

	public bool genBg;

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

	public int bholeChanceOffset;

	private System.Random rand;

	void Start () 
	{	
		if(instance == null)
		{
			instance = this;
		}
		rand = new System.Random();
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
				foregroundChunks.Add(posHash(xOff, yOff, 1f), generate(true, xOff, yOff));
				if(genBg)
				{
					backgroundChunks.Add(posHash(xOff, yOff, 1f), generate(false, xOff, yOff));
				}
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
			for(float yOff = -1 * genRadius * mult + yCenter; yOff < genRadius * mult + yCenter; yOff += areaHeight * mult)
			{
				if(signX > 0)
				{
					////////////////////////////////////
					//
					// Where further generation happens (POSITIVE X)
					//
					////////////////////////////////////
					destroyChunk(-1 * genRadius * mult + xCenter, yOff, true);
					foregroundChunks.Add(posHash(genRadius * mult + xCenter, yOff, mult), generate(true, genRadius * mult + xCenter, yOff));
					if(genBg)
					{
						destroyChunk(-1 * genRadius + xCenter, yOff, false);
						backgroundChunks.Add(posHash(genRadius * mult + xCenter, yOff, mult), generate(false, genRadius * mult + xCenter, yOff));
					}
				}
				else
				{
					////////////////////////////////////
					//
					// Where further generation happens (NEGATIVE X)
					//
					////////////////////////////////////
					destroyChunk(genRadius * mult + xCenter - areaWidth * mult, yOff, true);
					foregroundChunks.Add(posHash(-1 * genRadius * mult + xCenter - areaWidth * mult, yOff, mult), generate(true, -1 * genRadius * mult + xCenter - areaWidth * mult, yOff));
					if(genBg)
					{
						destroyChunk(genRadius * mult + xCenter - areaWidth * mult, yOff, false);
						backgroundChunks.Add(posHash(-1 * genRadius * mult + xCenter - areaWidth * mult, yOff, mult), generate(false, -1 * genRadius * mult + xCenter - areaWidth * mult, yOff));
					}
				}
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
			for(float xOff = -1 * genRadius * mult + xCenter; xOff < genRadius * mult + xCenter; xOff += areaWidth * mult)
			{
				if(signY > 0)
				{
					////////////////////////////////////
					//
					// Where further generation happens (POSITIVE Y)
					//
					////////////////////////////////////
					destroyChunk(xOff, -1 * genRadius * mult + yCenter, true);
					foregroundChunks.Add(posHash(xOff, genRadius * mult + yCenter, mult), generate(true, xOff, genRadius * mult + yCenter));
					if(genBg)
					{
						destroyChunk(xOff, -1 * genRadius * mult + yCenter, false);
						backgroundChunks.Add(posHash(xOff, genRadius * mult + yCenter, mult), generate(false, xOff, genRadius * mult + yCenter));
					}
				}
				else
				{
					////////////////////////////////////
					//
					// Where further generation happens (NEGATIVE Y)
					//
					////////////////////////////////////
					destroyChunk(xOff, genRadius * mult + yCenter - areaHeight * mult, true);
					foregroundChunks.Add(posHash(xOff, -1 * genRadius * mult + yCenter - areaHeight * mult, mult), generate(true, xOff, -1 * genRadius * mult + yCenter - areaHeight * mult));
					if(genBg)
					{
						destroyChunk(xOff, genRadius * mult + yCenter - areaHeight * mult, false);
						backgroundChunks.Add(posHash(xOff, -1 * genRadius * mult + yCenter - areaHeight * mult, mult), generate(false, xOff, -1 * genRadius * mult + yCenter - areaHeight * mult));
					}
				}
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

		float[][] asteroids = ProceduralGeneration.generate(areaWidth * size, areaHeight * size, minDensity / size, densityRange / size, genChance, minGenSize, genSizeRange, minGenSpacing,
		                                                    genSpacingRange, minAsteroidSize * size, asteroidSizeRange * size, sizeDistribution);
		List<int> ids = new List<int>();
		currentLayer += bholeChanceOffset;
		float curvePosition = ((float)(currentLayer - 1)) / ((float)currentLayer);

		float bhChance = blackHoleChance.Evaluate(curvePosition);
		bhChance = bhChance > 0 ? bhChance : 0;

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

	public void destroyChunk(float xOff, float yOff, bool isForeground)
	{
		float mult = ProgressCircle.SizeMultiplierFromLayer(ProgressCircle.instance.CurrentLayer);
		Dictionary<float, int[]> relevantChunks = isForeground ? foregroundChunks : backgroundChunks;
		ObjectPool relevantPool = isForeground ? foregroundPool : backgroundPool;
		int[] chunk = relevantChunks[posHash(xOff, yOff, mult)];
		foreach(int id in chunk)
		{
			relevantPool.removeBody(id);
		}
		relevantChunks.Remove(posHash(xOff, yOff, mult));
	}
	
	public float posHash(float xOff, float yOff, float mult)
	{
		return yOff * genRadius * 512f * mult + xOff;
	}


	public float radiusFromMass(float mass)
	{
		return mass * 0.08f;
	}

	/*------------------------------------------
	 * Causes the background layer of planets
	 * to be moved into the foreground layer.
	 * Empties background pool into foreground
	 * pool and foreground object pool into 
	 * background object pool.
	 * -----------------------------------------*/

	public void LayerUp()
	{ 
		float mult = ProgressCircle.SizeMultiplierFromLayer(ProgressCircle.instance.CurrentLayer);
		if(genBg)
		{
			// switch object pools
			ObjectPool tempPool = foregroundPool;
			foregroundPool = backgroundPool;
			backgroundPool = tempPool;
			backgroundPool.setEnabledChildren(false);
			foregroundPool.setEnabledChildren(true);

			backgroundPool.drain();

			//switch layers on objects
			foregroundPool.setPoolLayer(foregroundLayer.layer);
			foregroundPool.setParent (foregroundLayer);

			backgroundPool.setPoolLayer(backgroundLayer.layer);
			backgroundPool.setParent (backgroundLayer);

			// enable collisions for new foreground planets
			// and disable for background planets

			foregroundPool.setForegroundImage();

			foregroundPool.removePCOverlap(GameObject.Find ("PC").GetComponent<SpriteRenderer>().bounds);

			backgroundPool.setEnableCollisions(false);
			foregroundPool.setEnableCollisions(true);

			foregroundChunks = backgroundChunks;
			backgroundChunks = new Dictionary<float, int[]>();
			for(float yOff = -1 * genRadius * mult + yCenter ; yOff < genRadius * mult + yCenter; yOff += areaHeight)
			{
				for(float xOff = -1 * genRadius * mult + xCenter; xOff < genRadius * mult + xCenter; xOff += areaWidth * mult)
				{
					backgroundChunks.Add(posHash(xOff, yOff, mult), generate(false, xOff, yOff));	
				}
			}
		}
		else
		{
			foregroundPool.drain();
			foregroundChunks = new Dictionary<float, int[]>();
			for(float yOff = -1 * genRadius * mult + yCenter ; yOff < genRadius * mult + yCenter; yOff += areaHeight)
			{
				for(float xOff = -1 * genRadius * mult + xCenter; xOff < genRadius * mult + xCenter; xOff += areaWidth * mult)
				{
					foregroundChunks.Add(posHash(xOff, yOff, mult), generate(true, xOff, yOff));	
				}
			}
		}
	}

	public void LayerDown()
	{
		float mult = ProgressCircle.SizeMultiplierFromLayer(ProgressCircle.instance.CurrentLayer);
		if(genBg)
		{
			// switch object pools
			ObjectPool tempPool = foregroundPool;
			foregroundPool = backgroundPool;
			backgroundPool = tempPool;
			backgroundPool.setEnabledChildren(false);
			foregroundPool.setEnabledChildren(true);
			foregroundPool.drain();
			
			//switch layers on objects
			foregroundPool.setPoolLayer(foregroundLayer.layer);
			foregroundPool.setParent (foregroundLayer);
			
			backgroundPool.setPoolLayer(backgroundLayer.layer);
			backgroundPool.setParent (backgroundLayer);
			
			// enable collisions for new foreground planets
			// and disable for background planets
			
			foregroundPool.setForegroundImage();
			
			backgroundPool.setEnableCollisions(false);
			foregroundPool.setEnableCollisions(true);
			
			backgroundChunks = foregroundChunks;
			foregroundChunks = new Dictionary<float, int[]>();
			for(float yOff = -1 * genRadius + yCenter ; yOff < genRadius + yCenter; yOff += areaHeight * mult)
			{
				for(float xOff = -1 * genRadius + xCenter; xOff < genRadius + xCenter; xOff += areaWidth * mult)
				{
					foregroundChunks.Add(posHash(xOff, yOff, mult), generate(false, xOff, yOff));	
				}
			}
			foregroundPool.removePCOverlap(GameObject.Find ("PC").GetComponent<CircleCollider2D>().bounds);
		}
		else
		{
			foregroundPool.drain();
			foregroundChunks = new Dictionary<float, int[]>();
			for(float yOff = -1 * genRadius + yCenter ; yOff < genRadius + yCenter; yOff += areaHeight * mult)
			{
				for(float xOff = -1 * genRadius + xCenter; xOff < genRadius + xCenter; xOff += areaWidth * mult)
				{
					foregroundChunks.Add(posHash(xOff, yOff, mult), generate(true, xOff, yOff));	
				}
			}
		}
	}
}
