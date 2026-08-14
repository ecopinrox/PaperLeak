using Unity.Collections;
using UnityEngine;

public class CutscenePoint : Interactible
{
    [SerializeField] ConversationSO cutscene;

    ConversationRunner conversationRunner;

    private void Awake()
    {
        conversationRunner = FindAnyObjectByType<ConversationRunner>();
    }

    public override void Interact()
    {
        Debug.Log("interacted");
        //conversationRunner.StartConversation(cutscene);
    }
}
