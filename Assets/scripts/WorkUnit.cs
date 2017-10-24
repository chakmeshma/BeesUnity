using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public abstract class WorkUnit
{
    protected int threadWait = 17;
    protected Stopwatch stopwatch;
    protected Thread workerThread;
    protected bool repeating = false;
    protected float doneProgress = 0.0f;
	public bool finished = false;
	public Bee bee;
	public bool started = false;
	public abstract void start();
    public void stop()
    {
        finished = true;

        if(started)
        {
            workerThread.Abort();
            workerThread.Join();
        }

        lock (GameController.getInstance().beesActionLock)
        {
            List<System.Action> toDelete = new List<Action>();

            foreach (KeyValuePair<System.Action, WorkUnit> item in GameController.getInstance().beesActions)
            {
                if (item.Value == this)
                    toDelete.Add(item.Key);
            }

            foreach (System.Action key in toDelete)
            {
                try
                {
                    GameController.getInstance().beesActions.Remove(key);
                }
                catch (Exception e)
                {

                }
            }
        }
    }

    public void doWork()
    {
        while (true)
        {
            if (this.finished)
                break;

            Thread.Sleep(threadWait);

            System.Action workAction = new Action(doWorkPart);

            lock (GameController.getInstance().beesActionLock)
            {
                if (GameController.getInstance().beesActions.ContainsKey(workAction))
                    GameController.getInstance().beesActions.Remove(workAction);

                GameController.getInstance().beesActions.Add(workAction, this);
            }
        }
    }

    protected abstract void doWorkPart();
}