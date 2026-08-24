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
        onPlayerDiscovered += async () =>
        {
            Time.timeScale = 0f;
            LevelManager.Instance.RecordPlayerCaught();

            CartoonEffectManager cartoonEffectManager = FindAnyObjectByType<CartoonEffectManager>();
            if(cartoonEffectManager != null)
            {
                await cartoonEffectManager.ContractHole();
            }

            uiManager.SetGameOverPanelStatus(true);
        };
    }

    public void Save()
    {
        LevelManager.Instance.SaveLevelState();
        LevelManager.Instance.IncrementSaveCount();
    }

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
