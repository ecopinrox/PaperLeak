using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject challengesButton;
    [SerializeField] GameObject challengesPanel;
    [SerializeField] List<ChallengeUI> challenges;
    [SerializeField] TextMeshProUGUI minTimeText;
    [SerializeField] TextMeshProUGUI minSaveCountText;

    private void Start()
    {
        GameStats stats = LevelManager.Instance.GetOverallStats();
        if(stats == null)
        {
            challengesButton.SetActive(false);
        }
        else
        {
            UpdateChallengesPanel(stats);
        }

        challengesPanel.SetActive(false);
    }

    public void StartGame() => LevelManager.Instance.StartGame();

    public void ClearSave() => LevelManager.Instance.DeleteSave();

    public void SetChallengesPanelStatus(bool active)
    {
        challengesPanel.SetActive(active);
    }

    public void DeleteStats()
    {
        Debug.Log("delete overall stats");
        LevelManager.Instance.DeleteOverallStats();
    }

    void UpdateChallengesPanel(GameStats stats)
    {
        if(!stats.clearedWithoutChangingDifficulty)
        {
            challenges[0].SetStatus(true);
            challenges[1].SetStatus(false);
            challenges[2].SetStatus(false);
            challenges[3].SetStatus(false);
            challenges[4].SetStatus(false);
            challenges[5].SetStatus(false);
            challenges[6].SetStatus(false);
            challenges[7].SetStatus(false);

            minTimeText.text = $"minimum time taken: --:--:--";
            minSaveCountText.text = $"minimum save count: --";
        }
        else
        {
            challenges[0].SetStatus(true);
            challenges[1].SetStatus(stats.clearedWithoutChangingDifficulty);
            challenges[2].SetStatus(stats.clearedWithoutGettingCaught);
            challenges[3].SetStatus(stats.saveCount <= 0);
            challenges[4].SetStatus(stats.timeToBeat <= 40 * 60);
            challenges[5].SetStatus(stats.timeToBeat <= 30 * 60);
            challenges[6].SetStatus(stats.timeToBeat <= 20 * 60);
            challenges[7].SetStatus(stats.timeToBeat < 10 * 60 + 37);

            minTimeText.text = $"minimum time taken: {stats.GetTimeString()}";
            minSaveCountText.text = $"minimum save count: {stats.saveCount}";
        }
    }
}
