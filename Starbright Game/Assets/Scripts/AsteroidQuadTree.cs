using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{
	public class AsteroidQuadTree
	{
		private AsteroidQuadTree[] children = new AsteroidQuadTree[4];
		private float x;
		private float y;
		private float width;
		private float height;
		
		private bool terminal;
		
		private bool hasAsteroid;
		private float astX;
		private float astY;
		private float astSize;
		
		public AsteroidQuadTree (float leftX, float topY, float boundWidth, float boundHeight)
		{
			x = leftX;
			y = topY;
			width = boundWidth;
			height = boundHeight;
			
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
				return this;
			}
			
			AsteroidQuadTree child;
			
			switch(num)
			{
			case 0:
				child = new AsteroidQuadTree(x, y, width / 2, height / 2);
				break;
			case 1:
				child = new AsteroidQuadTree(x + width / 2, y, width / 2, height / 2);
				break;
			case 2:
				child = new AsteroidQuadTree(x, y + height / 2, width / 2, height / 2);
				break;
			case 3:
				child = new AsteroidQuadTree(x + width / 2, y + height / 2, width / 2, height / 2);
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
		
		public AsteroidQuadTree AddAsteroid(float asteroidX, float asteroidY, float asteroidSize)
		{
			if(!isInBounds(x, width, y, height, asteroidX, asteroidY))
			{
				return this;
			}
			if(IsTerminal && !HasAsteroid) 
			{
				hasAsteroid = true;
				astX = asteroidX;
				astY = asteroidY;
				astSize = asteroidSize;
				return this;
			}
			int child = PickChild(asteroidX, asteroidY);
			AddChild(child);
			children[child].AddAsteroid(asteroidX, asteroidY, asteroidSize);
			if(HasAsteroid)
			{
				child = PickChild(astX, astY);
				AddChild(child);
				children[child].AddAsteroid(astX, astY, asteroidSize);
			}
			return this;
		}
		
		public List<float[]> SearchWithinBounds(float leftX, float topY, float boundWidth, float boundHeight)
		{
			List<float[]> output = new List<float[]>();
			
			if(HasAsteroid)
			{
				output.Add(new float[]{astX, astY, astSize});
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
		
		public List<float[]> ToList()
		{
			List<float[]> output = new List<float[]>();
			if (hasAsteroid) 
			{
				output.Add(new float[]{astX, astY, astSize});
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
	}
}
