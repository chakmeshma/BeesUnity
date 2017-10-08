using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexController : MonoBehaviour {
	public Object hiveTilePrefab;
    public Object[] grassTilePrefabs;
    public Object[] flowerTilePrefabs;
    public int numberOfVerticalTiles;
    public int numberOfHorizontalTiles;
	public HiveTileAlignment hiveTileAlignment;
    public float tileSize = 1.0f;
    public float hiveSetChance;
    public float flowerSetChance;
    public TileController[][] tiles;
    private static HexController _instance;
    public float hiveCircleRadius = 2.0f;
    
    public static HexController getInstance()
    {
        return _instance;
    }

    public enum TileType
    {
        GrassTile,
        HiveTile,
        FlowerTile
    }


    public enum HiveTileAlignment {
		Table
	}
	// Use this for initialization
	void Start () {
		initHive ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Awake()
    {
        _instance = this;
    }

    private void initHive() {
        Hive hive = GameController.getInstance().hive;


        float tileAdjustentDistance = Mathf.Sqrt(3.0f);

        tiles = new TileController[numberOfVerticalTiles * 2 - 1][];
        for (int i = -numberOfVerticalTiles + 1; i < numberOfVerticalTiles; i++)
        {
            int index = i + numberOfVerticalTiles - 1;

            tiles[index] = new TileController[numberOfHorizontalTiles * 2 - 1];
        }


            for (int i = -numberOfVerticalTiles + 1; i < numberOfVerticalTiles; i++) {
            for (int j = -numberOfHorizontalTiles + 1; j < numberOfHorizontalTiles; j++)
            {
                GameObject tile = null;

                bool setHive = Random.Range(0.0f, 1.0f) < hiveSetChance;
                bool setFlower = Random.Range(0.0f, 1.0f) < flowerSetChance;

                Vector3 tilePosition = new Vector3(tileAdjustentDistance * i + (tileAdjustentDistance / 2.0f * (j % 2)), 0.0f, 1.5f * j);

                int indexI = i + numberOfVerticalTiles - 1;
                int indexJ = j + numberOfHorizontalTiles - 1;


                if (Mathf.Sqrt(Mathf.Pow(tilePosition.x, 2.0f) + Mathf.Pow(tilePosition.z, 2.0f)) < hiveCircleRadius)
                {
                    tile = Instantiate(hiveTilePrefab) as GameObject;

                    tile.GetComponent<HiveTileController>().init(indexI, indexJ, hive);

                    tiles[indexI][indexJ] = tile.GetComponent<HiveTileController>();
                }
                else
                {
                    if (setFlower) {
                        int flowerTileIndex = Random.Range(0, flowerTilePrefabs.Length);

                        tile = Instantiate(flowerTilePrefabs[flowerTileIndex]) as GameObject;

                        tile.GetComponent<FlowerTileController>().init(indexI, indexJ, 100, 100);


                        tiles[indexI][indexJ] = tile.GetComponent<FlowerTileController>();
                    }
                    else
                    {
                        int grassTileIndex = Random.Range(0, grassTilePrefabs.Length);

                        tile = Instantiate(grassTilePrefabs[grassTileIndex]) as GameObject;

                        tiles[indexI][indexJ] = tile.GetComponent<GrassTileController>();
                    }
                }

                tile.transform.localPosition = tilePosition;

            }
		}
	}
}
