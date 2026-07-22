using System;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    public static event Action OnInteractibleFound;
    public static event Action OnInteractibleCleared;

    Interactible currentInteractible;

    BoxCollider2D interactionTrigger;

    private void Awake()
    {
        interactionTrigger = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        //temporary - player faces away from the screen on game start
        SetDirection(Vector2.up);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.TryGetComponent(out Interactible interactible))
        {
            return;
        }

        currentInteractible = interactible;
        OnInteractibleFound?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!collision.TryGetComponent(out Interactible interactible))
        {
            return;
        }

        if (currentInteractible != interactible)
        {
            return;
        }

        currentInteractible = null;
        OnInteractibleCleared?.Invoke();
    }

    public void Interact()
    {
        if (currentInteractible == null)
        {
            return;
        }

        currentInteractible.Interact();
    }

    public void SetDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        interactionTrigger.offset = direction / 2;
    }
}
