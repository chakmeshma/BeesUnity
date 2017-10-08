using System.Collections.Generic;
using UnityEngine;

public class Hive
{
    public int maxHoney;
    public int honey;
    public List<Bee> bees;
    public string hiveName;
    public Color color;

    public Hive(string hiveName, Color color, int maxHoney, int initialHoney)
    {
        this.maxHoney = maxHoney;
        this.honey = initialHoney;

        bees = new List<Bee>();
        this.hiveName = hiveName;
        this.color = color;
    }

    public void updateBeesCurrentAction()
    {
        foreach (Bee bee in bees)
        {
            if (bee.workQueue.Count > 0)
            {
                if (!bee.workQueue[0].started)
                    bee.workQueue[0].start();
            }
        }
    }

    public void addBee(Bee bee)
    {
        bees.Add(bee);
    }
}