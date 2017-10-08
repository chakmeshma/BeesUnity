using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour {
    public Color color;
    public bool workQueueChanged = false;
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
    public List<WorkUnit> workQueue;
    public int maxQueueCapacity = 5;
    public string beeName;
    List<FlowerTileController> knownFlowers;

	public void init(BeeType type, string beeName, Color color)
    {
        this.type = type;
        this.beeName = beeName;
        this.color = color;

        workQueue = new List<WorkUnit>(maxQueueCapacity);
        workQueueChanged = true;
    }

    public MoveWorkUnit move(Vector3 destination) {
        MoveWorkUnit moveWorkUnit = new MoveWorkUnit(this, destination, true);

        return moveWorkUnit;
    }

    //public void OnMouseDown()
    //{
    //    GameController.getInstance().onBeeClicked(this);
    //}
}
