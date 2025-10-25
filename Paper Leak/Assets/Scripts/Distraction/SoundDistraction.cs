using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDistraction : Distraction 
{
    SoundManager soundManager;

    protected override void OnEnable()
    {
        base.OnEnable();
        soundManager = FindAnyObjectByType<SoundManager>();
    }

    public Coroutine PlaySound(SoundData sfx)
    {
        return soundManager.PlaySound(sfx, transform.position, this);
    }
}
