using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexController : MonoBehaviour {
	public Object hiveTilePrefab;
    public Object[] grassTilePrefabs;
	public int numberOfVerticalTiles;
    public int numberOfHorizontalTiles;
	public HiveTileAlignment hiveTileAlignment;
    public float tileSize = 1.0f;
    public float hiveSetChance;


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

	private void initHive() {
        float tileAdjustentDistance = Mathf.Sqrt(3.0f);


        for (int i = -numberOfVerticalTiles + 1; i < numberOfVerticalTiles; i++) {
            for (int j = -numberOfHorizontalTiles + 1; j < numberOfHorizontalTiles; j++)
            {
                GameObject tile = null;

                bool setHive = Random.Range(0.0f, 1.0f) < hiveSetChance;

                Vector3 tilePosition = new Vector3(tileAdjustentDistance * i + (tileAdjustentDistance / 2.0f * (j % 2)), 0.0f, 1.5f * j);

                if (Mathf.Sqrt(Mathf.Pow(tilePosition.x, 2.0f) + Mathf.Pow(tilePosition.z, 2.0f)) < 10.0f)
                    tile= Instantiate(hiveTilePrefab) as GameObject;
                else
                {
                    int grassTileIndex = Random.Range(0, grassTilePrefabs.Length);

                    tile = Instantiate(grassTilePrefabs[grassTileIndex]) as GameObject;
                }

                tile.transform.localPosition = tilePosition;

            }
		}
	}
}
