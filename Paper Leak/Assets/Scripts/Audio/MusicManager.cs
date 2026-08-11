using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioMixer musicMixer;

    int currentBgmId = 0;

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
}
