using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorkUnit
{
    protected bool repeating = false;
    protected double doneProgress = 0.0;
	public bool finished = false;
	public Bee bee;
	public bool started = false;
	public abstract void start();
}