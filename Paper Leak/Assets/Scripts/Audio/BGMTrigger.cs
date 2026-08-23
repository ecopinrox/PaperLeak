using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    MusicManager musicManager;

    [SerializeField] int bgmId;

    private void Awake()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
    }

    private void OnEnable()
    {
        MusicManager.OnBgmIdLoaded += DisableOnLoad;
    }

    private void OnDisable()
    {
        MusicManager.OnBgmIdLoaded -= DisableOnLoad;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            musicManager.SetBGMId(bgmId);
            gameObject.SetActive(false);
        }
    }

    void DisableOnLoad(int currentBgmId)
    {
        if(currentBgmId >= bgmId)
        {
            gameObject.SetActive(false);
        }
    }
}
