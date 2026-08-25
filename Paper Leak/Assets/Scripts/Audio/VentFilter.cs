using UnityEngine;

public class VentFilter : MonoBehaviour
{
    MusicManager musicManager;
    [SerializeField][Range(10f, 22000f)] float filterFrequency;

    private void Awake()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerController _))
        {
            musicManager.ActivateLowPassFilter(filterFrequency);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerController _))
        {
            musicManager.DeactivateLowPassFilter();
        }
    }
}
