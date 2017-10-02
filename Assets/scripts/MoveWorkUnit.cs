using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class MoveWorkUnit : WorkUnit
{
    private Thread workerThread;


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

    }
}