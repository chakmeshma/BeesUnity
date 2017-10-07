using System.Collections.Generic;

public class Hive
{
    private int maxHoney;
    private int initialHoney;
    private List<Bee> bees;

    public Hive(int maxHoney, int initialHoney)
    {
        this.maxHoney = maxHoney;
        this.initialHoney = initialHoney;

        bees = new List<Bee>();
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