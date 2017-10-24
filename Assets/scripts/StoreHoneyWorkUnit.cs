using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class StoreHoneyWorkUnit : WorkUnit
{
    private int buildTileIndexI;
    private int buildTileIndexJ;
    protected int threadWait = 120;

    public StoreHoneyWorkUnit(Bee selectedBee, int indexI, int indexJ, bool start)
    {
        this.bee = selectedBee;
        this.buildTileIndexI = indexI;
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
        workerThread.Name = "StoreHoneyWorkUnit Thread";
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

        Hive hive = HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<HiveTileController>().hive;

        int honeyTransfer = Mathf.RoundToInt(0.3f * (elapsedMilliseconds - lastTimeStamp));

        lastTimeStamp = elapsedMilliseconds;
        hive.honey += honeyTransfer;
        bee.honey -= honeyTransfer;

        if (bee.honey < 0)
        {
            int diff = -bee.honey;

            bee.honey += diff;
            hive.honey -= diff;
        }

        if (hive.honey > hive.maxHoney)
        {
            int diff = hive.honey - hive.maxHoney;
            bee.honey += diff;
            hive.honey -= diff;
        }

        if (hive.honey == hive.maxHoney || bee.honey == 0)
        {
            this.doneProgress = 1.0f;
            this.finished = true;
        }
    }
}