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
        gameObject.SetActive(false);
        FindAnyObjectByType<PlayerAnimation>().TurnInvisible();
        conversationRunner.StartConversation(cutscene);
        ConversationRunner.OnConversationEnd += () => { 
            Time.timeScale = 0f;
            _ = FindAnyObjectByType<CartoonEffectManager>().ContractHole();
        };
    }
}
