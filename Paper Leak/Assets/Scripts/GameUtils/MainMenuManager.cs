using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame() => LevelManager.Instance.StartGame();

    public void ClearSave() => LevelManager.Instance.DeleteSave();
}
