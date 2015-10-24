using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {

	[System.Serializable]
	public struct Coord 
	{
		public int x;
		public int y;
		
		public Coord(int _x, int _y) 
		{
			x = _x;
			y = _y;
		}
		
		public static bool operator ==(Coord c1, Coord c2)
		{
			return c1.x == c2.x && c1.y == c2.y;
		}
		
		public static bool operator !=(Coord c1, Coord c2)
		{
			return !(c1 == c2);
		}
		
	}

	public Transform tilePrefab;
	public int MapSizeX;
	public int MapSizeY;
	public float tileScale;
	Transform[,] tileMap;

	public void Start()
	{
		GenerateMap();
	}

	public void GenerateMap()
	{
		string holderName = "GeneratedMap";
		if(transform.FindChild(holderName))
		{
			DestroyImmediate(transform.FindChild(holderName).gameObject);
		}

		//transform.lossyScale = Vector3.one * tileScale;
		tileMap = new Transform[MapSizeX,MapSizeY];
		Transform mapHolder = new GameObject(holderName).transform;
		mapHolder.parent = this.transform;

		for (int x = 0;x  <MapSizeX ; x++)
		{
			for (int y = 0; y < MapSizeY; y++) {
				//Vector3 tilePosition = new Vector3(- MapSize.x/2 + tileScale/2 + x * tileScale, 0, - MapSize.y/2 + tileScale/2 + y * tileScale);
				Vector3 tilePosition = CoordToPosition(x, y);
				Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
				newTile.localScale = Vector3.one * tileScale;
				newTile.parent = mapHolder;
				tileMap[x,y] = newTile;
			}
		}

	}

	private Vector3 CoordToPosition(int x, int y)
	{
		return new Vector3 (-MapSizeX / 2f + 0.5f + x, 0, -MapSizeY / 2f + 0.5f + y) * tileScale;
	}

	public bool GetTileFromPosition(Vector3 position, out Transform tileTransf)
	{
		int x = Mathf.RoundToInt(position.x / tileScale + (MapSizeX - 1) / 2f);
		int y = Mathf.RoundToInt(position.z / tileScale + (MapSizeY - 1) / 2f);

		if(x > tileMap.GetLength(0) - 1 || y > tileMap.GetLength(1) - 1)
		{
			tileTransf = null;
			return false;
		}
		else
		{
			x = Mathf.Clamp (x, 0, tileMap.GetLength (0) -1);
			y = Mathf.Clamp (y, 0, tileMap.GetLength (1) -1);
			tileTransf = tileMap [x, y];
			return true;
		}
	}

}
