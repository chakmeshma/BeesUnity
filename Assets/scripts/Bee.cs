using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour {
    public int maxHP = 100;
    public int HP = 100;
    public int maxHoney = 30;
    public int honey = 0;
    public enum BeeType
    {
        Worker,
        Queen,
        Male
    }
    public BeeType type;
    public ArrayList workQueue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
