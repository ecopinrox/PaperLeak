using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioMixer musicMixer;

    [SerializeField] List<AudioSource> trackSources;
    [SerializeField] List<AudioLowPassFilter> trackFilters;
    [SerializeField][Range(0f, 1f)] float maxVolume;
    [SerializeField] List<float> interpDurationList = new();
    [SerializeField] List<AudioSource> oneShotTracks = new();

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
        for(int i = 0; i < trackSources.Count; i++)
        {
            trackFilters.Add(trackSources[i].GetComponent<AudioLowPassFilter>());
            trackFilters[i].cutoffFrequency = 22000f;
        }

        if(trackSources.Count != interpDurationList.Count)
        {
            Debug.LogError("Malformed lists in MusicManager");
        }

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

    public void SetBGMId(int id, bool oneShot)
    {
        if(oneShot)
        {
            PlayOneShotTrack(id);
        }

        if(currentBgmId >= id)
        {
            return;
        }

        SetActiveTrack(id);
    }

    public void ActivateLowPassFilter(float hz)
    {
        SetLowPassFilter(hz);
    }

    public void DeactivateLowPassFilter()
    {
        SetLowPassFilter(22000);
    }

    void SetLowPassFilter(float hz)
    {
        Debug.Log("Set low pass filter frequency to " + hz);
        foreach(AudioLowPassFilter filter in trackFilters)
        {
            filter.cutoffFrequency = hz;
        }
    }

    void SetActiveTrack(int id)
    {
        if(id >= trackSources.Count)
        {
            DisableAllTracks();
            return;
        }

        _ = LerpVolume(currentBgmId, id, interpDurationList[currentBgmId]);
        currentBgmId = id;
    }

    void PlayOneShotTrack(int id)
    {
        DisableAllTracks();
        oneShotTracks[id].volume = maxVolume;
        oneShotTracks[id].Play();
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

    void DisableAllTracks()
    {
        foreach(AudioSource track in trackSources)
        {
            track.volume = 0;
        }

        foreach(AudioSource track in oneShotTracks)
        {
            track.Stop();
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
        SetBGMId(save.bgmId, false);
        OnBgmIdLoaded?.Invoke(save.bgmId);
    }
}
