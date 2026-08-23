using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioMixer musicMixer;

    [SerializeField] List<AudioSource> trackSources;
    [SerializeField][Range(0f, 1f)] float maxVolume;
    [SerializeField] float interpDuration = 0.5f;

    public static event Action<int> OnBgmIdLoaded;

    int currentBgmId = 0;

    private void OnEnable()
    {
        LevelManager.OnStateSave += Save;
        LevelManager.OnStateLoad += Load;

        StartAllTracks();
    }

    private void OnDisable()
    {
        LevelManager.OnStateSave -= Save;
        LevelManager.OnStateLoad -= Load;
    }

    private void Start()
    {
        SetActiveTrack(currentBgmId);
    }

    void StartAllTracks()
    {
        foreach(AudioSource track in trackSources)
        {
            track.Play();
            track.volume = 0f;
        }

        trackSources[0].volume = maxVolume;
    }

    public void SetMusicVolume(float value)
    {
        float db = Mathf.Log10(value) * 20;
        musicMixer.SetFloat("Volume", db);
    }

    public float GetMusicVolume()
    {
        musicMixer.GetFloat("Volume", out float db);
        return Mathf.Pow(10, (db / 20));
    }

    public void SetBGMId(int id)
    {
        if(currentBgmId >= id)
        {
            return;
        }

        SetActiveTrack(id);
    }

    void SetActiveTrack(int id)
    {
        _ = LerpVolume(currentBgmId, id, interpDuration);
        currentBgmId = id;
    }

    async Awaitable LerpVolume(int oldId, int newId, float duration)
    {
        float time = 0;
        while(time < duration)
        {
            await Awaitable.FixedUpdateAsync();
            time += Time.fixedDeltaTime;

            //sine interpolation
            float t = CosInterp(Mathf.Clamp01(time / duration));
            float oldVol = maxVolume * (1 - t);
            float newVol = maxVolume * t;

            trackSources[oldId].volume = oldVol;
            trackSources[newId].volume = newVol;
        }
    }

    float CosInterp(float t)
    {
        return (1 - Mathf.Cos(Mathf.PI * t)) / 2;
    }

    void Save(SaveState save)
    {
        save.bgmId = currentBgmId;
    }

    void Load(SaveState save)
    {
        SetBGMId(save.bgmId);
        OnBgmIdLoaded?.Invoke(save.bgmId);
    }
}
