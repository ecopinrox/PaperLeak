using System;
using UnityEngine;

public class SoundDistraction : Distraction 
{
    SoundManager soundManager;

    protected override void OnEnable()
    {
        base.OnEnable();
        soundManager = FindAnyObjectByType<SoundManager>();
    }

    public void PlaySound(SoundData sfx)
    {
        SetPriority(sfx.priority);
        soundManager.PlaySound(sfx, transform.position, this);
    }
}
