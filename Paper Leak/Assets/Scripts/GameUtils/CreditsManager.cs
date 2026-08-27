using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;
    [SerializeField] int bpm = 130;
    [SerializeField] List<float> activationBeatList;

    [SerializeField] float outerPhaseTime;
    [SerializeField] float outerHoldTime;

    CartoonEffectManager cartoonEffectManager;
    AudioSource audioSource;

    private void Awake()
    {
        cartoonEffectManager = FindAnyObjectByType<CartoonEffectManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        foreach(GameObject p in panels)
        {
            p.SetActive(false);
        }

        if(panels.Count != activationBeatList.Count)
        {
            Debug.LogError("CreditsManager lists are malformed.");
        }
    }

    public async Awaitable StartCreditsSequence()
    {
        await cartoonEffectManager.ContractHole(outerPhaseTime, outerHoldTime);
        audioSource.Play();
        Time.timeScale = 1.0f;

        float time = 0f;
        for (int i = 0; i < activationBeatList.Count; )
        {
            float nextTransitionTime = activationBeatList[i] * (60f/bpm);
            if(time >= nextTransitionTime)
            {
                panels[i].SetActive(true);
                time -= nextTransitionTime;
                i++;
            }

            await Awaitable.FixedUpdateAsync();
            time += Time.fixedDeltaTime;
        }
    }
}

