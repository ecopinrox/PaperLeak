using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    MusicManager musicManager;
    PlayerInventory playerInventory;

    [SerializeField] int bgmId;
    [SerializeField] int itemIndex = -1;
    [SerializeField] int collectibleIndex = -1;

    private void Awake()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
        playerInventory = FindAnyObjectByType<PlayerInventory>();
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
        if(!collision.CompareTag("Player"))
        {
            return;
        }

        if(itemIndex >= 0 && !playerInventory.HasItem(itemIndex))
        {
            return;
        }

        if(collectibleIndex >= 0 && !playerInventory.HasCollectible(collectibleIndex))
        {
            return;
        }

        musicManager.SetBGMId(bgmId);
        gameObject.SetActive(false);
    }

    void DisableOnLoad(int currentBgmId)
    {
        if(currentBgmId >= bgmId)
        {
            gameObject.SetActive(false);
        }
    }
}
