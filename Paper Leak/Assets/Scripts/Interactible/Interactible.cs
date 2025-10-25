using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactible : MonoBehaviour
{
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController controller))
            controller.SetInteractible(this);
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController controller))
            controller.ClearInteractible();
    }

    public abstract void Interact(out bool uiEnabled);
}
