using System;
using System.Collections.Generic;
using UnityEngine;

public class ConversationRunner : MonoBehaviour
{
    IEnumerator<Dialogue> dialogueEnumerator;

    public static event Action OnConversationStart;
    public static event Action OnConversationEnd;

    UIManager uiManager;

    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
    }

    public void StartConversation(ConversationSO conversation)
    {
        dialogueEnumerator = conversation.GetEnumerator();

        OnConversationStart?.Invoke();
        uiManager.SetDialoguePanelStatus(true);

        AdvanceConversation();
    }

    public void AdvanceConversation()
    {
        if(dialogueEnumerator.MoveNext())
        {
            Dialogue current = dialogueEnumerator.Current;
            ShowDialogue(current);

            Debug.Log(
                $"BGM: {dialogueEnumerator.Current.bgmIndex}\n" +
                $"Actor index: {dialogueEnumerator.Current.actorIndex}\n" +
                $"Actor visual state: {dialogueEnumerator.Current.actorVisualState}\n" +
                $"Actor destination Y: {dialogueEnumerator.Current.actorDestination}\n" +
                $"Actor speed: {dialogueEnumerator.Current.actorSpeed}\n"
            );
        }
        else
        {
            EndConversation();
        }
    }

    void EndConversation()
    {
        OnConversationEnd?.Invoke();
        uiManager.SetDialoguePanelStatus(false);

        dialogueEnumerator.Dispose();
    }

    void ShowDialogue(Dialogue dialogue)
    {
        uiManager.ShowDialogue(dialogue);
    }
}
