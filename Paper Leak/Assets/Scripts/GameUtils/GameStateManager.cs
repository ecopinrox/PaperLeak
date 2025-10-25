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

    public void Save()          => LevelManager.Instance.Save();
    public void ReloadLevel()   => LevelManager.Instance.ReloadLevel();
    public void RestartLevel()  => LevelManager.Instance.RestartLevel();
}
