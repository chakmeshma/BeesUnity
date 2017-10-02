using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    private static GameController instance;
    public enum GameState
    {
        HIVE_SELECTED,
        BEE_SELECTED
    }
    public GameState state;
    public Bee selectedBee;

    void Awake()
    {
        instance = this;
    }

    public static GameController getInstance()
    {
        return instance;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onTileClicked(Tile tile)
    {

    }
}
