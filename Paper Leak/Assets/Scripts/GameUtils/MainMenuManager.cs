using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject challengesButton;

    private void Start()
    {
        if(!LevelManager.Instance.CheckForOverallStats())
        {
            challengesButton.SetActive(false);
        }
    }

    public void StartGame() => LevelManager.Instance.StartGame();

    public void ClearSave() => LevelManager.Instance.DeleteSave();

    public void ShowChallenges()
    {
        Debug.Log("Show challenges");
    }
}
