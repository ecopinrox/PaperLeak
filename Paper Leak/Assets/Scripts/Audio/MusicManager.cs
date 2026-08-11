using System;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioMixer musicMixer;

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

        currentBgmId = id;
        Debug.Log(currentBgmId);
    }

    void Save(SaveState save)
    {
        save.bgmId = currentBgmId;
    }

    void Load(SaveState save)
    {
        currentBgmId = save.bgmId;
        Debug.Log("Loaded BGM ID " + currentBgmId);
        OnBgmIdLoaded?.Invoke(currentBgmId);
    }
}
