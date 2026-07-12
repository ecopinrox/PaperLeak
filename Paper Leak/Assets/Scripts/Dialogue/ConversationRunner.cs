using System;
using System.Collections.Generic;
using UnityEngine;

public class ConversationRunner : MonoBehaviour
{
    IEnumerator<Dialogue> dialogueEnumerator;

    public static event Action OnConversationStart;
    public static event Action OnConversationEnd;

    public void StartConversation(ConversationSO conversation)
    {
        dialogueEnumerator = conversation.GetEnumerator();

        OnConversationStart?.Invoke();
        AdvanceConversation();
    }

    public void AdvanceConversation()
    {
        if(dialogueEnumerator.MoveNext())
        {
            ShowDialogue(dialogueEnumerator.Current);
        }
        else
        {
            EndConversation();
        }
    }

    void EndConversation()
    {
        Debug.Log("conversation over");
        OnConversationEnd?.Invoke();

        dialogueEnumerator.Dispose();
    }

    void ShowDialogue(Dialogue dialogue)
    {
        Debug.Log(dialogue.text);
    }
}
