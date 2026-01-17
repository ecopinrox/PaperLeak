using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] SaveState saveState;
    public static SaveState SaveState { get { return Instance.saveState; } }

    public static int currentDifficultySetting = 1;

    public float TimeElapsed { get; private set; } = 0;

    private void Awake()
    {
        if(Instance != null) Destroy(gameObject);
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
        Save();
    }

    private void Update()
    {
        TimeElapsed += Time.deltaTime;
    }

    void LoadDifficultySettings()
    {
        DifficultySwitch.loadDifficultySettings(currentDifficultySetting);
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(string levelName)
    {
        TimeElapsed = 0;
        SceneManager.LoadScene(levelName);
    }

    public void RestartLevel() 
    {
        LoadLevel(SceneManager.GetActiveScene().name);
        StartCoroutine(ExecuteAfterDelay(LoadDifficultySettings));
    }

    IEnumerator ExecuteAfterDelay(Action action)
    {
        yield return null;
        action?.Invoke();
    }

    public void Save()
    {

    }

    public void SetDifficulty(int difficulty)
    {
        currentDifficultySetting = difficulty;
        LoadDifficultySettings();
    }
}
