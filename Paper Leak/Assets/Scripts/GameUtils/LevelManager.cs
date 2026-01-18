using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] SaveState saveState;

    public static Action<SaveState> OnStateLoad;
    public static Action<SaveState> OnStateSave;

    //0 = easy, 1 = normal, 2 = hard
    public static int currentDifficultySetting = 1;

    public float TimeElapsed { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        LoadDifficultySettings();

        //Clear save
        SaveLevelState();
    }

    private void Update()
    {
        TimeElapsed += Time.deltaTime;
    }

    void LoadDifficultySettings()
    {
        DifficultySwitch.loadDifficultySettings(currentDifficultySetting);
    }

    public async Awaitable LoadLevel(string levelName)
    {
        TimeElapsed = 0;
        SceneManager.LoadScene(levelName);

        await Awaitable.EndOfFrameAsync();
        SaveLevelState();
    }

    public async Awaitable RestartLevel() 
    {
        _ = LoadLevel(SceneManager.GetActiveScene().name);
        
        await Awaitable.EndOfFrameAsync();

        LoadDifficultySettings();
    }

    public void SaveLevelState()
    {
        OnStateSave?.Invoke(saveState);
    }

    public async Awaitable LoadLevelState()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        await Awaitable.NextFrameAsync();

        OnStateLoad?.Invoke(saveState);
        LoadDifficultySettings();
    }

    public void SetDifficulty(int difficulty)
    {
        currentDifficultySetting = difficulty;
        LoadDifficultySettings();
    }
}
