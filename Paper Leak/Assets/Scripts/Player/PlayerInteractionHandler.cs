using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    Interactible currentInteractible;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.TryGetComponent(out Interactible interactible))
        {
            return;
        }

        currentInteractible = interactible;
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
    }

    public bool Interact()
    {
        return currentInteractible.Interact();
    }
}
