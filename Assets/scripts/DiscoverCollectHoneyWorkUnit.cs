using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Diagnostics;

public class DiscoverCollectHoneyWorkUnit : MoveWorkUnit
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private FlowerTileController firstFlower = null;
    protected float endHeight = GameController.getInstance().beeFlowerCollectHeight;
    private bool addedCollectWorkUnit = false;

    public DiscoverCollectHoneyWorkUnit(Bee bee, bool start) : base(bee, Vector3.zero, start)
    {
        this.bee = bee;
        this.stopwatch = Stopwatch.StartNew();

        if (start)
        {
            this.start();
        }
    }

    public override void start()
    {
        workerThread = new Thread(new ThreadStart(doWork));
        workerThread.Start();

        started = true;

        this.startPosition = bee.transform.position;
    }

    public void end()
    {
        try
        {
            workerThread.Abort();
        }
        catch (Exception e)
        {

        }
    }

    private long lastTimeStamp = -1;

    protected override void doWorkPart()
    {
        if (firstFlower == null)
        {
            for (int i = -HexController.getInstance().numberOfVerticalTiles + 1; i < HexController.getInstance().numberOfVerticalTiles; i++)
            {
                if (firstFlower != null)
                    break;

                for (int j = -HexController.getInstance().numberOfHorizontalTiles + 1; j < HexController.getInstance().numberOfHorizontalTiles; j++)
                {
                    if (firstFlower != null)
                        break;

                    int indexI = i + HexController.getInstance().numberOfVerticalTiles - 1;
                    int indexJ = j + HexController.getInstance().numberOfHorizontalTiles - 1;

                    TileController tile = HexController.getInstance().tiles[indexI][indexJ];

                    if (tile is FlowerTileController && tile.getState() == TileController.TileState.Visible && ((FlowerTileController)tile).honey > 0)
                    {
                        firstFlower = (FlowerTileController)tile;
                    }

                    if (firstFlower != null)
                        break;
                }

                if (firstFlower != null)
                    break;
            }
        }

        if(firstFlower == null)
        {
            this.doneProgress = 1.0f;

            if (this.doneProgress >= 1.0f)
            {
                this.doneProgress = 1.0f;
                this.finished = true;
            }

            return;
        }

        endPosition = new Vector3(firstFlower.transform.position.x, endHeight, firstFlower.transform.position.z);

        if (!addedCollectWorkUnit)
        {
            this.bee.workQueue.Add(new CollectHoneyWorkUnit(this.bee, firstFlower.indexI, firstFlower.indexJ, false));
            this.bee.workQueueChanged = true;


            addedCollectWorkUnit = true;
        }

        if (endPosition == startPosition)
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

        float speedMultiplier = (endPosition - startPosition).magnitude;

        this.doneProgress += 0.0035f * (elapsedMilliseconds - lastTimeStamp) / speedMultiplier;

        lastTimeStamp = elapsedMilliseconds;

        if (this.doneProgress >= 1.0f)
        {
            this.doneProgress = 1.0f;
            this.finished = true;
        }

        Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, (float)doneProgress);

        try
        {
            bee.transform.position = newPosition;
            bee.transform.LookAt(new Vector3(endPosition.x, bee.transform.position.y, endPosition.z));

            if (GameController.getInstance().state == GameController.GameState.BEE_SELECTED)
                GameController.getInstance().visibilitiesChanged = true;
        }
        catch (Exception e)
        {

        }

    }
}