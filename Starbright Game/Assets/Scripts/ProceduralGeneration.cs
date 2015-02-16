using UnityEngine;
using System;
using System.Collections.Generic;
public class ProceduralGeneration
{
	private static System.Random rand = new System.Random();
	private static float poissonCellSize = 25f;
	private static float minDistance = 5f;
	private static int maxAttempts = 20;
	private static Dictionary<ulong, AsteroidQuadTree> poissonSamples = new Dictionary<ulong, AsteroidQuadTree>();

	public static AsteroidQuadTree generate(float areaWidth, float areaHeight, float minDensity, float densityRange, int minGenSize, int genSizeRange, float minGenSpacing, float genSpacingRange, float minAsteroidSize, float asteroidSizeRange, float multiplier, float multiplierJitter, float finalSize, float finalSizeJitter, AnimationCurve sizeDistribution, float bHoleChance, int samplePoints, float chunkX, float chunkY)
	{
		AsteroidQuadTree asteroids = new AsteroidQuadTree(0, 0, areaWidth, areaHeight, minAsteroidSize + 2 * asteroidSizeRange);


		//Generate number of seed asteroids
		int numSeeds = (int)Math.Round(minDensity + rand.NextDouble() * densityRange);

		for (int i = 0; i < numSeeds; i++) 
		{
			Asteroid sample = pickSample(chunkX, chunkY);
			float seedX;
			float seedY;
			float seedSize;
            //Generate seed asteroid position
			if(sample != null)
			{
				int attempts = 0;
				bool failed = false;
				do
				{
					double angle = rand.NextDouble() * 2 * Math.PI;
					double dist = (rand.NextDouble() * minDistance) + minDistance;
					seedX = sample.X + (float)(dist * Math.Cos(angle));
					seedY = sample.Y + (float)(dist * Math.Sin(angle));
					int baseX = (int)Math.Floor(seedX / poissonCellSize);
					int baseY = (int)Math.Floor(seedY / poissonCellSize);
					ulong hash = posHash(baseX,baseY);
					if(!poissonSamples.ContainsKey(hash))
						poissonSamples[hash] = new AsteroidQuadTree(baseX, baseY, poissonCellSize, poissonCellSize, Generator.massFromRadius(minDistance * 2));
					if(!poissonSamples[hash].AddAsteroid(new Asteroid(seedX, seedY, minDistance)))
						failed = true;
					attempts++;
				}while(seedX < chunkX || seedX > chunkX + areaWidth || seedY < chunkY || seedY > chunkY + areaHeight || failed || attempts < maxAttempts);
				if(attempts >= maxAttempts)
				{
					continue;
				}
				seedX -= chunkX;
				seedY -= chunkY;
			}
			else
			{
				seedX = (float)rand.NextDouble() * areaWidth;
				seedY = (float)rand.NextDouble() * areaHeight;
			}
			//Pick a size for the asteroid
            seedSize = minAsteroidSize + (sizeDistribution.Evaluate((float)rand.NextDouble())) * asteroidSizeRange;

			float mult = multiplier + (float)(0.5 - rand.NextDouble()) * multiplierJitter;
			float final = finalSize + (float)(0.5 - rand.NextDouble()) * finalSizeJitter;

			bool isBHole = rand.NextDouble() < bHoleChance;

			//Grow further generations out from the seed asteroid
			List<Asteroid> group = growSeed (seedX, seedY, seedSize, isBHole, minGenSize, genSizeRange, minGenSpacing, genSpacingRange, minAsteroidSize, asteroidSizeRange, mult, final, sizeDistribution, bHoleChance);
			asteroids.AddList(group);
		}
		return asteroids;
	}

	//Recursively build a cluster from a seed point
	public static List<Asteroid> growSeed(float x, float y, float size, bool isBHole, int minGenSize, int genSizeRange, float minGenSpacing, float genSpacingRange, float minAsteroidSize, float asteroidSizeRange, float multiplier, float finalSize, AnimationCurve sizeDistribution, float bHoleChance)
	{
		List<Asteroid> asteroids = new List<Asteroid>();

		//Add seed asteroid to the group
		asteroids.Add (new Asteroid(x, y, size, isBHole));

		//Check if we should do a further generation. If not, return the current list (just the seed)
		float newSize = minAsteroidSize * multiplier;
		if(newSize < finalSize)
		{
			return asteroids;
		}
		float newSizeRange = asteroidSizeRange * multiplier;

		//decide how many asteroids to spawn
		int genSize = minGenSize + rand.Next (genSizeRange);
		for (int i = 0; i < genSize; i++)
		{
			//Generate position of child relative to parent
            float distance = (minGenSpacing + (float)rand.NextDouble() * genSpacingRange) * Generator.radiusFromMass(size);
			double angle = rand.NextDouble() * 2 * Math.PI;

			//Convert to cartesian
			float nextX = x + distance * (float)Math.Cos(angle);
			float nextY = y + distance * (float)Math.Sin(angle);

			//Add the child asteroid
			float nextSize = newSize + (sizeDistribution.Evaluate((float)(1 - rand.NextDouble()))) * newSizeRange;

			bool nextBHole = rand.NextDouble() < bHoleChance;

			//Use the child asteroid as a new seed, possibly producing its own children
			asteroids.AddRange(growSeed(nextX, nextY, nextSize, nextBHole, minGenSize, genSizeRange, minGenSpacing, genSpacingRange, newSize, newSizeRange, multiplier, finalSize, sizeDistribution, bHoleChance));
		}
		//Debug.Log ("Grow " + asteroids.Count);
		return asteroids;
	}

	private static ulong posHash(int x, int y)
	{
		ulong newX = (uint)x;
		ulong newY = (uint)y;
		return (newY << 32) | newX;
	}

	private static Asteroid pickSample(float x, float y)
	{
		int baseX = (int)Math.Floor(x / poissonCellSize);
		int baseY = (int)Math.Floor(y / poissonCellSize);
		List<AsteroidQuadTree> candidates= new List<AsteroidQuadTree>();
		for(int xOff = -2; xOff <= 2; xOff++)
		{
			for(int yOff = -2; yOff <= 2; yOff++)
			{
				ulong hash = posHash(baseX + xOff, baseY + yOff);
				if(poissonSamples.ContainsKey(hash) && !poissonSamples[hash].isEmpty())
				{
					candidates.Add(poissonSamples[hash]);
				}
			}
		}
		if(candidates.Count == 0)
		{
			return null;
		}
		List<Asteroid> container = candidates[rand.Next(candidates.Count)].ToList();
		return container[rand.Next(container.Count)];
	}
}
