using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class ConversationRunner : MonoBehaviour
{
    IEnumerator<Dialogue> dialogueEnumerator;

    public static event Action OnConversationStart;
    public static event Action OnConversationEnd;

    readonly List<CutsceneActor> actorList = new();
    int runningTasks = 0;

    UIManager uiManager;
    MusicManager musicManager;

    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
        musicManager = FindAnyObjectByType<MusicManager>();
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
        if (runningTasks > 0) return;

        if (dialogueEnumerator.MoveNext())
        {
            Dialogue current = dialogueEnumerator.Current;
            ShowDialogue(current);

            if(current.bgmIndex is int bgmId)
            {
                if(bgmId >= 0)
                {
                    musicManager.SetBGMId(bgmId, false);
                }
                else
                {
                    musicManager.SetBGMId(Mathf.Abs(bgmId) - 1, true);
                }
            }

            foreach (string actorState in current.actorStates)
            {
                string[] values = actorState.Split(',');

                int index = int.Parse(values[0]);

                runningTasks++;
                actorList[index].SetState(
                    TryParseNullableInt(values[1]),
                    TryParseNullableInt(values[2]),
                    TryParseFloat(values[3], 3.5f),
                    DecrementRunningTasks
                );
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

    int? TryParseNullableInt(string tag)
    {
        if (int.TryParse(tag, out int value)) return value;
        return null;
    }

    float TryParseFloat(string tag, float defaultValue)
    {
        if(float.TryParse(tag, out float value)) return value;
        return defaultValue;
    }

    void DecrementRunningTasks()
    {
        if(runningTasks > 0)
        {
            runningTasks--;
        }
        else
        {
            runningTasks = 0;
        }
    }
}
