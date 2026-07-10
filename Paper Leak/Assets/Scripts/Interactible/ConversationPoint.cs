using UnityEngine;

public class ConversationPoint : Interactible
{
    bool interacted = false;

    [SerializeField] Color interactedColor;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Interact()
    {
        if (!interacted)
        {
            Debug.Log("conversation start");
            interacted = true;
            SwitchColor();
        }
        else
        {
            Debug.Log("conversation start again");
        }
    }

    void SwitchColor()
    {
        spriteRenderer.color = interactedColor;
    }
}
