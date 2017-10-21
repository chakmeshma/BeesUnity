using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour {
    private static DebugController _instance;
    public static DebugController getInstance()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayDebugText(string text) {
        transform.Find("Debug Text").GetComponent<UnityEngine.UI.Text>().text = text;
    }

    public void DisplayAppendDebugText(string text)
    {
        transform.Find("Debug Text").GetComponent<UnityEngine.UI.Text>().text += "\n" + text;

    }
}
