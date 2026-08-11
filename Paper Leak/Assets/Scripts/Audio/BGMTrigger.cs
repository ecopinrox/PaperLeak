using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    MusicManager musicManager;

    [SerializeField] int bgmId;

    private void Awake()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        musicManager.SetBGMId(bgmId);
        gameObject.SetActive(false);
    }
}
