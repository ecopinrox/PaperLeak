using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public delegate void OnPlayerDiscovered();
    public static OnPlayerDiscovered onPlayerDiscovered;

    UIManager uiManager;

    void Awake()
    {
        uiManager = GetComponent<UIManager>();

        onPlayerDiscovered = null;
        onPlayerDiscovered += () =>
        {
            uiManager.SetGameOverPanelStatus(true);
            Time.timeScale = 0f;
        };
    }

    public void Save()          => LevelManager.Instance.SaveLevelState();
    public void ReloadLevel()   => _ = LevelManager.Instance.ReloadLevelState();
    public void RestartLevel()  => _ = LevelManager.Instance.RestartLevel();
}
