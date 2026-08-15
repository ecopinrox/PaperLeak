using System;
using System.Collections.Generic;
using UnityEngine;

public class ConversationRunner : MonoBehaviour
{
    IEnumerator<Dialogue> dialogueEnumerator;

    public static event Action OnConversationStart;
    public static event Action OnConversationEnd;

    readonly List<CutsceneActor> actorList = new();

    UIManager uiManager;

    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
    }

    public void StartConversation(ConversationSO conversation)
    {
        dialogueEnumerator = conversation.GetEnumerator();

        for (int i = 0; i < conversation.ActorPrefabs.Count; i++)
        {
            CutsceneActor actor = Instantiate(
                conversation.ActorPrefabs[i], 
                (Vector2)conversation.ActorStartPos[i], 
                Quaternion.identity, 
                transform
            ).GetComponent<CutsceneActor>();

            actorList.Add(actor);
        }

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

            string debug = $"BGM: {current.bgmIndex}\n";
            Debug.Log(debug);

            foreach(string actorState in current.actorStates)
            {
                string[] values = actorState.Split(',');
                int index = int.Parse(values[0]);
                actorList[index].SetState(actorState);
            }
        }
        else
        {
            EndConversation();
        }
    }

    void EndConversation()
    {
        actorList.Clear();

        OnConversationEnd?.Invoke();
        uiManager.SetDialoguePanelStatus(false);

        dialogueEnumerator.Dispose();
    }

    void ShowDialogue(Dialogue dialogue)
    {
        uiManager.ShowDialogue(dialogue);
    }
}
