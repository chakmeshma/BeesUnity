using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Diagnostics;

public class DiscoverWorkUnit : WorkUnit
{
    private Thread workerThread;
    private Vector3 startPosition;
    private Vector3 endPosition;
	private Stopwatch stopwatch;

    public DiscoverWorkUnit(Bee bee, bool start)
    {
        this.bee = bee;
		this.stopwatch = Stopwatch.StartNew ();

        if(start)
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
        } catch(Exception e)
        {

        }
    }

    public void doWork()
    {
        while(true)
        {
            if (this.finished)
                break;

            Thread.Sleep(23);

            System.Action workAction = new Action(doWorkPart);

            lock(GameController.getInstance().beesActionLock)
            {
				if (GameController.getInstance ().beesActions.ContainsKey (workAction))
					GameController.getInstance ().beesActions.Remove (workAction);

                GameController.getInstance().beesActions.Add(workAction, this);
            }
        }
    }

	private long lastTimeStamp = -1;

    private void doWorkPart()
    {
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
		
		this.doneProgress += 0.0005f * (elapsedMilliseconds - lastTimeStamp) / speedMultiplier;

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
        }
        catch (Exception e)
        {

        }

    }
}