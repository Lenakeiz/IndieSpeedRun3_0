using UnityEngine;
using System.Collections;

public class MapController : MonoBehaviour {

	public Transform tilePrefab;
	public Vector2 MapSize;
	public float tileScale;

	public void GenerateMap()
	{
		string holderName = "GeneratedMap";
		if(transform.FindChild(holderName)){
			DestroyImmediate(transform.FindChild(holderName).gameObject);
		}

		Transform mapHolder = new GameObject(holderName).transform;
		mapHolder.parent = this.transform;

		for (int x = 0;x  < MapSize.x; x++) {
			for (int y = 0; y < MapSize.y; y++) {
				Vector3 tilePosition = new Vector3(- MapSize.x/2 + tileScale/2 + x * tileScale, 0, - MapSize.y/2 + tileScale/2 + y * tileScale);
				Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
				newTile.localScale = Vector3.one * tileScale;
				newTile.parent = mapHolder;
			}
		}
	}

}
