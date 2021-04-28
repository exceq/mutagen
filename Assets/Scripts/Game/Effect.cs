using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    public float StartTime { get; set; }
    public float Duration { get; private set; }
    public Action Action { get; private set; }


    public Effect(float duration, Action action)
    {
        this.Duration = duration;
        this.Action = action;
    }

    public void Execute()
    {
        StartTime = Time.time;
        Action();
    }
}
