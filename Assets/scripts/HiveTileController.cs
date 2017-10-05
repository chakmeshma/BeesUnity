using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveTileController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        GameController.getInstance().state = GameController.GameState.HIVE_SELECTED;    }
}
