using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileGenerator : MonoBehaviour {
    public Object[] mapTilePrefab;
    public int mapSize = 3;
    public float tileSize = 2.0f;

	// Use this for initialization
	void Start () {
        initMap();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void initMap()
    {
        float realTileSize = 10.0f * tileSize;
		Random random = new Random ();

        for(int i = (int)(-mapSize / 2.0f); i <= (int)(mapSize / 2.0f); i++)
        {
            for (int j = (int)(-mapSize / 2.0f); j <= (int)(mapSize / 2.0f); j++)
            {
				int mapTileRandomIndex = Mathf.RoundToInt (Random.Range (0.0f, mapTilePrefab.Length - 1.0f));

				GameObject mapTile = Instantiate(mapTilePrefab[mapTileRandomIndex]) as GameObject;

				mapTile.transform.localPosition = new Vector3 (tileSize * i, 0.0f, tileSize * j);
				mapTile.transform.localScale = new Vector3(tileSize, tileSize, tileSize);

            }
        }
    }
}
