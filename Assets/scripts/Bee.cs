using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour {
    public Color color;
    public bool workQueueChanged = false;
    public int maxHP = 100;
    public int HP = 100;
    public int maxHoney = 1000;
    public int honey = 0;
    public enum BeeType
    {
        Worker,
        Queen,
        Male
    }
    public BeeType type;
    public List<WorkUnit> workQueue;
    public int maxQueueCapacity = 5;
    public string beeName;
    public HashSet<TileController> visitedTiles;

    public void init(BeeType type, string beeName, Color color)
    {
        this.type = type;
        this.beeName = beeName;
        this.color = color;

        visitedTiles = new HashSet<TileController>();

        workQueue = new List<WorkUnit>(maxQueueCapacity);
        workQueueChanged = true;
    }
}
