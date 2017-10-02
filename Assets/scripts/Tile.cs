using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public int positionX, positionZ;
    public TileType type;
    public enum TileType
    {
        HiveTile,
        BareTile,
        MainHiveTile
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onChildClicked()
    {
        GameController.getInstance().onTileClicked(this);
    }
}
