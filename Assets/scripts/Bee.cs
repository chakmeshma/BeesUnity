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
    public int maxQueueCapacity = 5;
    public string beeName;

    public Bee(BeeType type, string beeName)
    {
        this.type = type;
        this.beeName = beeName;

        workQueue = new ArrayList(maxQueueCapacity);
    }

    public MoveWorkUnit move(Vector3 destination) {
        MoveWorkUnit moveWorkUnit = new MoveWorkUnit(this, this.transform.position, destination, true);

        return moveWorkUnit;
    }

    public void OnMouseDown()
    {
        GameController.getInstance().onBeeClicked(this);
    }
}
