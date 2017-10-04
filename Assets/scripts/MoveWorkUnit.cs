using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class MoveWorkUnit : WorkUnit
{
    private Thread workerThread;
    private Vector3 startPosition;
    private Vector3 endPosition;
    public Bee bee;


    public MoveWorkUnit(Bee bee, Vector3 startPosition, Vector3 endPosition, bool start)
    {
        this.bee = bee;
        this.startPosition = startPosition;
        this.endPosition = endPosition;

        if(start)
        {
            this.start();
        }
    }

    public void start()
    {
        workerThread = new Thread(new ThreadStart(doWork));
        workerThread.Start();
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
            Thread.Sleep(20);

            this.doneProgress += 0.01f;

            bee.transform.position = Vector3.Lerp(startPosition, endPosition, (float)doneProgress);
        }
    }
}