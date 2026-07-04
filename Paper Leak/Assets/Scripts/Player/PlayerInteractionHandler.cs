using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
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
