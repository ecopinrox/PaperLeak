using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;
    [SerializeField] List<float> activationTimeList;

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

        if(panels.Count != activationTimeList.Count)
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
        for (int i = 0; i < activationTimeList.Count; )
        {
            if(time >= activationTimeList[i])
            {
                panels[i].SetActive(true);
                i++;
            }

            await Awaitable.FixedUpdateAsync();
            time += Time.fixedDeltaTime;
        }
    }
}

