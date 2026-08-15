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

            string debug = $"BGM: {dialogueEnumerator.Current.bgmIndex}\n";
            foreach(string state in current.actorStates)
            {
                debug += $"state: {state}\n";
            }
            Debug.Log(debug);
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
