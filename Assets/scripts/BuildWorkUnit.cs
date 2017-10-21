using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class BuildWorkUnit : WorkUnit
{
    private int buildTileIndexI;
    private int buildTileIndexJ;

    public BuildWorkUnit(Bee selectedBee, int indexI, int indexJ, bool start)
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
        try { this.doneProgress = HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<HiveBuildTileController>().progress; }
        catch (Exception e)
        {
            bee.workQueue.Remove(this);
            GameController.getInstance().workQueueChangedFlag = true;

            if (started)
                stop();

            return;
        }


        workerThread = new Thread(new ThreadStart(doWork));
        workerThread.Name = "BuildWorkUnit Thread";
        workerThread.Start();

        started = true;
    }

    public void doWork()
    {
        while (true)
        {
            if (this.finished)
                break;

            Thread.Sleep(16);

            System.Action workAction = new Action(doWorkPart);

            lock (GameController.getInstance().beesActionLock)
            {
                if (GameController.getInstance().beesActions.ContainsKey(workAction))
                    GameController.getInstance().beesActions.Remove(workAction);

                GameController.getInstance().beesActions.Add(workAction, this);
            }
        }
    }

    private long lastTimeStamp = -1;

    private void doWorkPart()
    {
        if (finished)
            return;

        if ((new Vector2(bee.transform.position.x - HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].transform.position.x,
                        bee.transform.position.z - HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].transform.position.z)).magnitude > 0.300f)
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

        this.doneProgress += 0.00035f * (elapsedMilliseconds - lastTimeStamp);

        lastTimeStamp = elapsedMilliseconds;

        if (this.doneProgress >= 1.0f)
        {
            this.doneProgress = 1.0f;
            this.finished = true;

            Hive hive = HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<HiveBuildTileController>().hive;

            GameObject newHiveTile = GameObject.Instantiate(GameController.getInstance().hiveTilePrefab, HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].transform.position, Quaternion.identity) as GameObject;

            newHiveTile.GetComponent<HiveTileController>().init(buildTileIndexI, buildTileIndexJ, hive);

            HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<HiveBuildTileController>().progress = this.doneProgress;

            GameObject.Destroy(HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].gameObject);

            HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ] = newHiveTile.GetComponent<HiveTileController>();
        }

        try
        {
            HexController.getInstance().tiles[buildTileIndexI][buildTileIndexJ].GetComponent<HiveBuildTileController>().progress = this.doneProgress;
        }catch (Exception e)
        {

        }
    }
}