using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Diagnostics;

public class MoveWorkUnit : WorkUnit
{
    private Vector3 startPosition;
    private Vector3 endPosition;

    public MoveWorkUnit(Bee bee, Vector3 endPosition, bool start)
    {
        this.bee = bee;
        this.endPosition = endPosition;
		this.stopwatch = Stopwatch.StartNew ();

        if(start)
        {
            this.start();
        }
    }

    public override void start()
    {
        if (!started)
        {
            workerThread = new Thread(new ThreadStart(doWork));
            workerThread.Name = "MoveWorkUnit Thread";
            workerThread.Start();

            started = true;
        }

        this.startPosition = bee.transform.position;
    }

    public void doWork()
    {
        while(true)
        {
            if (this.finished)
                break;

            Thread.Sleep(10);

            System.Action workAction = new Action(doWorkPart);

            lock(GameController.getInstance().beesActionLock)
            {
                try
                {
                    GameController.getInstance().beesActions.Add(workAction, this);
                } catch (Exception e)
                {
                }
            }
        }
    }

	private long lastTimeStamp = -1;

    private void doWorkPart()
    {
        if (finished)
            return;

        if(endPosition == startPosition)
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

        if(this.doneProgress >= 1.0f)
        {
            this.doneProgress = 1.0f;
            this.finished = true;
        }

        Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, (float)doneProgress);

        try
        {
            bee.transform.position = newPosition;

            if(GameController.getInstance().state == GameController.GameState.BEE_SELECTED)
                GameController.getInstance().visibilitiesChanged = true;
        }
        catch (Exception e)
        {

        }

    }
}