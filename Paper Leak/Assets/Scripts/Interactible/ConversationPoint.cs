using Unity.Collections;
using UnityEngine;

public class ConversationPoint : Interactible
{
    bool interacted = false;

    [SerializeField] ConversationSO conversation;
    [SerializeField] Color interactedColor;

    SpriteRenderer spriteRenderer;

    ConversationRunner conversationRunner;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        conversationRunner = FindAnyObjectByType<ConversationRunner>();
    }

    public override void Interact()
    {
        if (!interacted)
        {
            interacted = true;
            SwitchColor();
            Debug.Log("conversation start");
        }
        else
        {
            Debug.Log("conversation start again");
        }

        conversationRunner.StartConversation(conversation);
    }

    void SwitchColor()
    {
        spriteRenderer.color = interactedColor;
    }
}
