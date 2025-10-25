using System.Collections;
using UnityEngine;

public class PopPopController : MonoBehaviour
{
    DistractionMovement movement;
    SoundDistraction soundDistraction;

    [SerializeField] SoundData sfx;

    private void OnEnable()
    {
        soundDistraction = GetComponent<SoundDistraction>();
    }

    public void SetDestination(Vector2Int destination, bool isCrawling)
    {
        movement = GetComponent<DistractionMovement>();

        movement.OnReached += () => 
        {
            soundDistraction.PlaySound(sfx);
            gameObject.SetActive(false);
        };

        movement.SetDestination(destination, isCrawling);
    }
}
