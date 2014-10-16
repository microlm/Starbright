using UnityEngine;
using System;
using System.Collections.Generic;
public class ProceduralGeneration
{
	static System.Random rand = new System.Random();

	/// <summary>
	/// Creates an array of ordered pairs detailing asteroid placement based on generation parameters.
	/// 
	/// In depth description:
	/// First, a number from minSeeds to minSeeds + seedsRange is picked. This will be how many seed asteroids are placed.
	/// Then, for each seed:
	/// A random position within the area is picked (x is less than area width, y is less than area height)
	/// An asteroid is added at that position. Then based on genChance, it is decided whether or not a new generation will spawn from the asteroid.
	/// Each spawned asteroid goes through the same process as its seed until no new generations are spawned, and all seeds have been generated.
	/// An array of all of the asteroid positions is returned.
	/// </summary>
	/// <param name="areaWidth">Area width.</param>
	/// <param name="areaHeight">Area height.</param>
	/// <param name="minSeeds">Minimum number of seeds.</param>
	/// <param name="seedsRange">How much seed number can vary.</param>
	/// <param name="genChance">Chance of an asteroid spawning another generation, between 0 and 1.</param>
	/// <param name="minGenSize">Minimum number of asteroids in a generation.</param>
	/// <param name="genSizeRange">How much generation size can vary</param>
	/// <param name="minGenSpacing">Minimum distance between an asteroid and its parent.</param>
	/// <param name="genSpacingRange">How much the distance varies.</param>
    /// <param name="minAsteroidSize">Minimum asteroid size.</param>
    /// <param name="asteroidSizeRange">How much the asteroid size varies.</param>
	public static float[][] generate(float areaWidth, float areaHeight, float minDensity, float densityRange,
	                               double genChance, int minGenSize, int genSizeRange, float minGenSpacing,
	                               float genSpacingRange, float minAsteroidSize, float asteroidSizeRange,
	                               Gradient sizeDistribution)
	{
		AsteroidQuadTree asteroids = new AsteroidQuadTree(0, 0, areaWidth, areaHeight, minAsteroidSize + asteroidSizeRange);

		//Generate number of seed asteroids
		int minSeeds = (int)(areaWidth * areaHeight * minDensity);
		int seedsRange = (int)(areaWidth * areaHeight * densityRange);
		int numSeeds = minSeeds + rand.Next (seedsRange);

		for (int i = 0; i < numSeeds; i++) 
		{
            //Generate seed asteroid position
			float seedX = (float)rand.NextDouble() * areaWidth;
			float seedY = (float)rand.NextDouble() * areaHeight;


			//Pick a size for the asteroid
            float seedSize = minAsteroidSize + (1 - sizeDistribution.Evaluate((float)rand.NextDouble()).grayscale) * asteroidSizeRange;

			//Grow further generations out from the seed asteroid
			List<float[]> group = growSeed (seedX, seedY, seedSize, genChance, minGenSize, genSizeRange, minGenSpacing, genSpacingRange, minAsteroidSize, asteroidSizeRange, sizeDistribution);
			asteroids.AddList(group);
		}
		return asteroids.ToList().ToArray();
	}

	//Recursively build a cluster from a seed point
	public static List<float[]> growSeed(float x, float y, float size, double genChance, int minGenSize,
                                   int genSizeRange, float minGenSpacing, float genSpacingRange,
                                   float minAsteroidSize, float asteroidSizeRange, Gradient sizeDistribution)
	{
		List<float[]> asteroids = new List<float[]>();

		//Add seed asteroid to the group
		asteroids.Add (new float[]{x, y, size});

		//Check if we should do a further generation. If not, return the current list (just the seed)
		double shouldGrow = rand.NextDouble ();
	
		if(shouldGrow < (1 - genChance))
		{
			return asteroids;
		}

		//decide how many asteroids to spawn
		int genSize = minGenSize + rand.Next (genSizeRange);
		for (int i = 0; i < genSize; i++)
		{
			//Generate position of child relative to parent
            float distance = minGenSpacing + (float)rand.NextDouble() * genSpacingRange;
			double angle = rand.NextDouble() * 2 * Math.PI;

			//Convert to cartesian
			float nextX = x + distance * (float)Math.Cos(angle);
			float nextY = y + distance * (float)Math.Sin(angle);

			//Add the child asteroid
			float nextSize = minAsteroidSize + (1 - sizeDistribution.Evaluate((float)(1 - rand.NextDouble())).grayscale) * asteroidSizeRange;

			//Use the child asteroid as a new seed, possibly producing its own children
			asteroids.AddRange(growSeed(nextX, nextY, nextSize, genChance, minGenSize, genSizeRange, minGenSpacing, genSpacingRange, minAsteroidSize, asteroidSizeRange, sizeDistribution));
		}
		Debug.Log ("Grow " + asteroids.Count);
		return asteroids;
	}
	
}

