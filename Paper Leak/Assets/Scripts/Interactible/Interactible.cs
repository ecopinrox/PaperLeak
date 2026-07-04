using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactible : MonoBehaviour
{
    public abstract bool Interact();
}
