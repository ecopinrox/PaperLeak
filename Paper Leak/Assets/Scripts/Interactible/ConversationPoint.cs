using Unity.Collections;
using UnityEngine;

public class ConversationPoint : Interactible
{
    bool interacted = false;

    [SerializeField] ConversationSO mainConversation;
    [SerializeField] ConversationSO subConversation;
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
        ConversationSO conversation;

        if (!interacted)
        {
            interacted = true;
            SwitchColor();
            conversation = mainConversation;
        }
        else
        {
            conversation = subConversation;
        }

        conversationRunner.StartConversation(conversation);
    }

    void SwitchColor()
    {
        spriteRenderer.color = interactedColor;
    }
}
