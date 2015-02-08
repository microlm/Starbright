using System;
public class Asteroid
{
	private float x;
	private float y;
	private float size;
	private bool isBHole;
	private bool isRemoved;
	
	public Asteroid(float astX, float astY, float astSize, bool bHole = false)
	{
		x = astX;
		y = astY;
		size = astSize;
		isBHole = bHole;
	}
	
	public float X
	{
		get { return x; }
	}
	
	public float Y
	{
		get { return y; }
	}
	
	public float Size
	{
		get { return size; }
	}
	
	public bool IsBhole
	{
		get { return isBHole; }
	}
	
	public bool IsRemoved
	{
		get { return isRemoved; }
		set { isRemoved = value; }
	}
}