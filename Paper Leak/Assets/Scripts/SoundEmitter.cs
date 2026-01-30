using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(CircleCollider2D))]
public class SoundEmitter : MonoBehaviour
{
    [SerializeField] LayerMask blockSoundMask;
    AudioSource audioSource;
    CircleCollider2D circleCollider;
    Distraction originDistraction;

    public event Action OnComplete;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;
    }

    public Coroutine PlaySound(SoundData soundData, Distraction originDistraction)
    {
        List<AudioClip> list = soundData.audioClips;
        AudioClip clip = list[UnityEngine.Random.Range(0, list.Count)];

        if (soundData.distractionRadius > Mathf.Epsilon)
        {
            circleCollider.radius = soundData.distractionRadius;
        }
        else
        {
            circleCollider.enabled = false;
        }

        audioSource.PlayOneShot(clip);
        this.originDistraction = originDistraction;
        circleCollider.enabled = true;

        return StartCoroutine(InvokeOnClipEnd(clip.length));
    }

    public Distraction GetOriginDistraction() => originDistraction;

    public float GetDistractionRadius()
    {
        return circleCollider.radius;
    }
    
    IEnumerator InvokeOnClipEnd(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        circleCollider.enabled = false;
        OnComplete?.Invoke();
    }
}
