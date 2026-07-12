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

    Vector2Int GridPos { get { return Vector2Int.RoundToInt(transform.position); } }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        conversationRunner = FindAnyObjectByType<ConversationRunner>();
    }

    private void OnEnable()
    {
        LevelManager.OnStateSave += Save;
        LevelManager.OnStateLoad += Load;
    }

    private void OnDisable()
    {
        LevelManager.OnStateSave -= Save;
        LevelManager.OnStateLoad -= Load;
    }

    public override void Interact()
    {
        ConversationSO conversation;

        if (!interacted)
        {
            MarkAsInteracted();
            conversation = mainConversation;
        }
        else
        {
            conversation = subConversation;
        }

        conversationRunner.StartConversation(conversation);
    }

    void MarkAsInteracted()
    {
        interacted = true;
        SwitchColor();
    }

    void SwitchColor()
    {
        spriteRenderer.color = interactedColor;
    }

    void Save(SaveState saveState)
    {
        if(interacted)
        {
            saveState.interactedConversationPoints.Add(GridPos);
        }
        else
        {
            saveState.interactedConversationPoints.Remove(GridPos);
        }
    }

    void Load(SaveState saveState)
    {
        if(saveState.interactedConversationPoints.Contains(GridPos))
        {
            MarkAsInteracted();
        }
    }
}
