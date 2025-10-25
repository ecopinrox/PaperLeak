using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] GameObject soundEmitter;

    readonly Stack<GameObject> soundPool = new();

    public Coroutine PlaySound(SoundData data, Vector2 position, Distraction source)
    {
        SoundEmitter emitter = GetOrAddSoundEmitter(position);
        return emitter.PlaySound(data, source);
    }

    SoundEmitter GetOrAddSoundEmitter(Vector2 position)
    {
        GameObject soundObject;

        if (soundPool.Count == 0)
        {
            soundObject = Instantiate(soundEmitter, position, Quaternion.identity, transform);
            SoundEmitter emitter = soundObject.GetComponent<SoundEmitter>();
            emitter.OnComplete += () => { ReturnToPool(emitter.gameObject); };
        }
        else
        {
            soundObject = soundPool.Pop();
            soundObject.transform.position = position;
            soundObject.SetActive(true);
        }

        return soundObject.GetComponent<SoundEmitter>();
    }

    void ReturnToPool(GameObject sound)
    {
        sound.SetActive(false);
        soundPool.Push(sound);
    }
}
