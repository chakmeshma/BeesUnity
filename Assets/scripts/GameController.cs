using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    private static GameController instance;
    public enum GameState
    {
        NAVIGATION,
        HIVE_SELECTED,
        BEE_SELECTED
    }
    public GameState state = GameState.NAVIGATION;
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

    public void onBeeClicked(Bee bee)
    {
        switch(state)
        {
            case GameState.NAVIGATION:
                this.selectedBee = bee;
                this.state = GameState.BEE_SELECTED;

                updateUI();
                break;
        }
    }

    private void updateUI() {
        switch(state)
        {
            case GameState.BEE_SELECTED:
                UIController.getInstance().showBeeCP(selectedBee);
                break;
        }
    }
}
