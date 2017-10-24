using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class CollectHoneyWorkUnit : WorkUnit
{
    private int buildTileIndexI;
    private int buildTileIndexJ;
    protected int threadWait = 120;

    public CollectHoneyWorkUnit(Bee selectedBee, int indexI, int indexJ, bool start)
    {
        this.bee = selectedBee;
        this.buildTileIndexI= indexI;
        this.buildTileIndexJ = indexJ;

        this.stopwatch = Stopwatch.StartNew();


        if (start)
        {
            this.start();
        }
    }

    public override void start()
    {
        workerThread = new Thread(new ThreadStart(doWork));
        workerThread.Name = "CollectHoneyWorkUnit Thread";
        workerThread.Start();

        started = true;
    }

    private long lastTimeStamp = -1;

    protected override void doWorkPart()
    {
        if (finished)
            return;

        if ((new Vector2(bee.transform.position.x - HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].transform.position.x,
                        bee.transform.position.z - HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].transform.position.z)).magnitude > 5.000f)
        {
            this.doneProgress = 1.0f;

            if (this.doneProgress >= 1.0f)
            {
                this.doneProgress = 1.0f;
                this.finished = true;
            }

            return;
        }


        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds; 

        if (lastTimeStamp == -1)
            lastTimeStamp = elapsedMilliseconds;

        int honeyTransfer =  Mathf.RoundToInt(0.3f * (elapsedMilliseconds - lastTimeStamp));

        lastTimeStamp = elapsedMilliseconds;
        HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<FlowerTileController>().honey -= honeyTransfer;
        bee.honey += honeyTransfer;

        if(bee.honey > bee.maxHoney)
        {
            int diff = bee.honey - bee.maxHoney;
            bee.honey -= diff;
            HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<FlowerTileController>().honey += diff;
        }

        if(HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<FlowerTileController>().honey < 0)
        {
            int diff = -HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<FlowerTileController>().honey;

            HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<FlowerTileController>().honey += diff;
            bee.honey -= diff;
        }

        if (HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<FlowerTileController>().honey == 0 || bee.honey == bee.maxHoney)
        {
            this.doneProgress = 1.0f;
            this.finished = true;
        }
    }
}