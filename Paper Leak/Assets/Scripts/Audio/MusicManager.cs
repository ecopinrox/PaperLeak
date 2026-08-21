using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioMixer musicMixer;

    [SerializeField] List<AudioSource> trackSources;
    [SerializeField][Range(0f, 1f)] float maxVolume;

    public static event Action<int> OnBgmIdLoaded;

    int currentBgmId = 0;

    private void OnEnable()
    {
        LevelManager.OnStateSave += Save;
        LevelManager.OnStateLoad += Load;
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
        currentBgmId = id;
        for (int i = 0; i < trackSources.Count; i++)
        {
            trackSources[i].volume = (i == currentBgmId) ? maxVolume : 0f;
        }
    }

    void Save(SaveState save)
    {
        save.bgmId = currentBgmId;
    }

    void Load(SaveState save)
    {
        OnBgmIdLoaded?.Invoke(save.bgmId);
    }
}
