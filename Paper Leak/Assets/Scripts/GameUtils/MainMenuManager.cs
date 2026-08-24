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
    }

    void UpdateChallengesPanel(GameStats stats)
    {
        challenges[0].SetStatus(true);
        challenges[1].SetStatus(stats.clearedWithoutChangingDifficulty);
        challenges[2].SetStatus(stats.clearedWithoutGettingCaught);
        challenges[3].SetStatus(stats.saveCount <= 0);
        challenges[4].SetStatus(stats.timeToBeat <= 40 * 60);
        challenges[5].SetStatus(stats.timeToBeat <= 30 * 60);
        challenges[6].SetStatus(stats.timeToBeat <= 20 * 60);
        challenges[7].SetStatus(stats.timeToBeat <= 13 * 60 + 28);

        minTimeText.text = $"minimum time taken: {TimeSpan.FromSeconds(stats.timeToBeat):hh\\:mm\\:ss}";
        minSaveCountText.text = $"minimum save count: {stats.saveCount}";
    }
}
