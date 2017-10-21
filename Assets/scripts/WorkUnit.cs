using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public abstract class WorkUnit
{
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
}