using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    public float StartTime { get; set; }
    public float Duration { get; private set; }
    public Action Action { get; private set; }
    public Action PostAction { get; private set; }

    public Effect(float duration, Action action, Action PostAction)
    {
        this.Duration = duration;
        this.Action = action;
        this.PostAction = PostAction;
    }
}
