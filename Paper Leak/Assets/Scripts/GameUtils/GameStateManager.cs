using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public delegate void OnPlayerDiscovered();
    public static OnPlayerDiscovered onPlayerDiscovered;

    public bool Paused { get; private set; } = false;

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

    public void Save()              => LevelManager.Instance.SaveLevelState();
    public void ReloadLevel()       => _ = LevelManager.Instance.ReloadLevelState();
    public void RestartLevel()      => _ = LevelManager.Instance.ReloadLevelFromScratch();
    public void ReturnToMainMenu()  => LevelManager.Instance.ReturnToMainMenu();

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Paused = true;
        uiManager.SetPausePanelStatus(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Paused = false;
        uiManager.SetPausePanelStatus(false);
    }
}
