using UnityEngine;
using System.Collections;

public class Tile
{

	public int xPos { private set; get; }

	public int yPos { private set; get; }

	public Tile (int x, int y)
	{
		xPos = x;
		yPos = y;
	}

}