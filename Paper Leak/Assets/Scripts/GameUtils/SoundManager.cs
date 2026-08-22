using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] GameObject soundEmitter;
    [SerializeField][Range(0f, 1f)] float maxVolume;
    [SerializeField] SFXVolume volumeSO;

    public static SoundManager Instance { get; private set; }

    readonly Stack<GameObject> soundPool = new();

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine PlaySound(SoundData data, Vector2 position, Distraction source)
    {
        SoundEmitter emitter = GetOrAddSoundEmitter(position);
        return emitter.PlaySound(data, source, volumeSO.volume);
    }

    public void SetSFXVolume(float volume)
    {
        volumeSO.volume = volume * maxVolume;
    }

    public float GetSFXVolume()
    {
        return volumeSO.volume / maxVolume;
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
