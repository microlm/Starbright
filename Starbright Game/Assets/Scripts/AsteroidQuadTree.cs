using System;
using System.Collections.Generic;
public class AsteroidQuadTree
{
	private AsteroidQuadTree[] children = new AsteroidQuadTree[4];
	private float x;
	private float y;
	private float width;
	private float height;
	
	private bool terminal;
	
	private bool hasAsteroid;
	private Asteroid childAsteroid;
	private float maxSize;
	
	public AsteroidQuadTree (float leftX, float topY, float boundWidth, float boundHeight, float maxAstSize)
	{
		x = leftX;
		y = topY;
		width = boundWidth;
		height = boundHeight;
		maxSize = maxAstSize;
		
		terminal = true;
		hasAsteroid = false;
	}
	
	public bool HasChild(int num)
	{
		return children[num] != null;
	}
	
	public AsteroidQuadTree GetChild(int num)
	{
		return children[num];
	}
	
	public AsteroidQuadTree AddChild(int num)
	{
		if(HasChild(num))
		{
			return children[num];
		}
		
		AsteroidQuadTree child;
		
		switch(num)
		{
		case 0:
			child = new AsteroidQuadTree(x, y, width / 2, height / 2, maxSize);
			break;
		case 1:
			child = new AsteroidQuadTree(x + width / 2, y, width / 2, height / 2, maxSize);
			break;
		case 2:
			child = new AsteroidQuadTree(x, y + height / 2, width / 2, height / 2, maxSize);
			break;
		case 3:
			child = new AsteroidQuadTree(x + width / 2, y + height / 2, width / 2, height / 2, maxSize);
			break;
		default:
			child = null;
			break;
		}
		
		children[num] = child;
		terminal = false;
		return this;
	}
	
	public bool IsTerminal
	{
		get { return terminal; }
	}
	
	public bool HasAsteroid
	{
		get { return hasAsteroid; }
	}

	public bool HasBlackHole
	{
		get { return hasAsteroid && childAsteroid.IsBhole; }
	}
	
	public int PickChild (float pointX, float pointY)
	{
		bool isLeft = pointX < x + width / 2;
		bool isTop = pointY < y + height / 2;
		if(isTop)
		{
			if(isLeft)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}
		else
		{
			if(isLeft)
			{
				return 2;
			}
			else
			{
				return 3;
			}
		}
	}

	public bool AddAsteroid(Asteroid asteroid)
	{
		if(Generator.radiusFromMass(asteroid.Size) > maxSize)
		{
			return false;
		}
		float range = Generator.radiusFromMass(asteroid.Size) + maxSize;
		foreach(Asteroid a in SearchWithinBounds(asteroid.X - range, asteroid.Y - range, range * 2, range * 2))
		{
			if(doesIntersect(asteroid, a))
			{
				return false;
			}
		}
		return AddAsteroidHelper(asteroid);
	}

	public bool AddAsteroidHelper(Asteroid asteroid)
	{
		if(!isInBounds(x, y, width, height, asteroid.X, asteroid.Y))
		{
			return false;
		}
		if(IsTerminal && !HasAsteroid) 
		{
			hasAsteroid = true;
			childAsteroid = asteroid;
			return true;
		}
		int child;
		if(HasAsteroid)
		{
			child = PickChild(childAsteroid.X, childAsteroid.Y);
			AddChild(child);
			children[child].AddAsteroidHelper(childAsteroid);
			hasAsteroid = false;
		}
		child = PickChild(asteroid.X, asteroid.Y);
		AddChild(child);
		children[child].AddAsteroidHelper(asteroid);
		return true;
	}

	public AsteroidQuadTree AddList(List<float[]> asteroids)
	{
		foreach(float[] a in asteroids)
		{
			if(a.Length == 3)
			{
				AddAsteroid(new Asteroid(a[0], a[1], a[2]));
			}
			else
			{
				throw new ArgumentException("Asteroid data did not have 3 elements");
			}
		}
		return this;
	}

	public AsteroidQuadTree AddList(List<Asteroid> asteroids)
	{
		foreach(Asteroid a in asteroids)
		{
			AddAsteroid(a);
		}
		return this;
	}

	public Asteroid SearchAt(float x, float y)
	{
		if(HasAsteroid && childAsteroid.X == x && childAsteroid.Y == y)
		{
			return childAsteroid;
		}
		int child = PickChild(x,y);
		if(HasChild(child))
			return children[child].SearchAt(x,y);
		return null;
	}
	
	public List<Asteroid> SearchWithinBounds(float leftX, float topY, float boundWidth, float boundHeight)
	{
		List<Asteroid> output = new List<Asteroid>();
		
		if(HasAsteroid)
		{
			output.Add(childAsteroid);
			return output;
		}
		
		if(HasChild(0) && doesIntersect(x, y, width / 2, height / 2, leftX, topY, boundWidth, boundHeight))
		{
			output.AddRange(children[0].SearchWithinBounds(leftX, topY, boundWidth, boundHeight));
		}
		if(HasChild(1) && doesIntersect(x + width / 2, y, width / 2, height / 2, leftX, topY, boundWidth, boundHeight))
		{
			output.AddRange(children[1].SearchWithinBounds(leftX, topY, boundWidth, boundHeight));
		}
		if(HasChild(2) && doesIntersect(x, y + height / 2, width / 2, height / 2, leftX, topY, boundWidth, boundHeight))
		{
			output.AddRange(children[2].SearchWithinBounds(leftX, topY, boundWidth, boundHeight));
		}
		if(HasChild(3) && doesIntersect(x + width / 2, y + height / 2, width / 2, height / 2, leftX, topY, boundWidth, boundHeight))
		{
			output.AddRange(children[3].SearchWithinBounds(leftX, topY, boundWidth, boundHeight));
		} 
		return output;
	}
	
	public List<Asteroid> ToList()
	{
		List<Asteroid> output = new List<Asteroid>();
		if (hasAsteroid) 
		{
			output.Add(childAsteroid);
		}
		for(int i = 0; i < 4; i++)
		{
			if(HasChild(i))
			{
				output.AddRange(children[i].ToList());
			}
		}
		return output;
	}

	public bool isEmpty()
	{
		return (IsTerminal && !HasAsteroid);
	}

	private static bool isInBounds(float start, float length, float point)
	{
		return (point >= start) && (point < start + length);
	}
	
	private static bool isInBounds(float leftX, float topY, float boundWidth, float boundHeight, float pointX, float pointY)
	{
		return isInBounds(leftX, boundWidth, pointX) && isInBounds(topY, boundHeight, pointY);
	}
	
	private static bool doesIntersect(float leftX, float topY, float boundWidth, float boundHeight, float rectX, float rectY, float rectWidth, float rectHeight)
	{
		if ((isInBounds (leftX, boundWidth, rectX) || isInBounds (leftX, boundWidth, rectX + rectWidth)) &&
		    (isInBounds (topY, boundHeight, rectY) || isInBounds (topY, boundHeight, rectY + rectHeight)))
		{
			return true;
		}
		if ((isInBounds (rectX, rectWidth, leftX) || isInBounds (rectX, rectWidth, leftX + boundWidth)) &&
		    (isInBounds (rectY, rectHeight, topY) || isInBounds (rectY, rectHeight, topY + boundHeight)))
		{
			return true;
		}
		return false;
	}

	public static bool doesIntersect(Asteroid a, Asteroid b)
	{
		return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y)) < Generator.radiusFromMass(a.Size) + Generator.radiusFromMass(b.Size);
	}
}

