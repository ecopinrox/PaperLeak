using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] MasterSave masterSave;
    [field: SerializeField] public ItemIndexer ItemIndexer { get; private set; }

    enum LevelLoadOptions { JsonLoad, SkipLoad, SaveOnLoad };
    [SerializeField] LevelLoadOptions levelLoadOption;

    [SerializeField] int landmineIndex;

    [SerializeField] string saveFileName;

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

    private void OnEnable()
    {
        //landmine loading
        OnStateLoad += LoadLandmines;

        OnStateSave += SaveDifficulty;
        OnStateLoad += LoadDifficulty;

        OnStateSave += SaveTimeElapsed;
        OnStateLoad += LoadTimeElapsed;
    }

    private void OnDisable()
    {
        OnStateLoad -= LoadLandmines;

        OnStateSave -= SaveDifficulty;
        OnStateLoad -= LoadDifficulty;

        OnStateSave -= SaveTimeElapsed;
        OnStateLoad -= LoadTimeElapsed;
    }

    private void Start()
    {
        LoadDifficultySettings();

        if (levelLoadOption == LevelLoadOptions.JsonLoad)
        {
            try
            {
                JsonSaver.Load(masterSave, saveFileName);
            }
            catch(FileNotFoundException)
            {
                SaveLevelState();
            }

            _ = LoadLevelState();
        }
        else if (levelLoadOption == LevelLoadOptions.SaveOnLoad)
        {
            SaveLevelState();
        }
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
        SaveState saveState = masterSave.GetCurrentLevelState();
        masterSave.visited.Add(masterSave.currentLevelIndex);

        OnStateSave?.Invoke(saveState);
        JsonSaver.Save(masterSave, saveFileName);
    }

    public async Awaitable ReloadLevelState()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        await LoadLevelState();
    }

    public async Awaitable LoadLevelState()
    {
        await Awaitable.NextFrameAsync();

        SaveState saveState = masterSave.GetCurrentLevelState();
        OnStateLoad?.Invoke(saveState);
        LoadDifficultySettings();
    }

    public void SetDifficulty(int difficulty)
    {
        currentDifficultySetting = difficulty;
        LoadDifficultySettings();
    }

    void LoadLandmines(SaveState saveState)
    {
        foreach (Vector2Int loc in saveState.mineLocations)
        {
            Instantiate(ItemIndexer.GetItem(landmineIndex), (Vector2)loc, Quaternion.identity);
        }
    }

    void SaveDifficulty(SaveState saveState)
    {
        masterSave.difficulty = currentDifficultySetting;
    }

    void LoadDifficulty(SaveState saveState)
    {
        currentDifficultySetting = masterSave.difficulty;
    }

    void SaveTimeElapsed(SaveState saveState)
    {
        saveState.timeElapsed = TimeElapsed;
    }

    void LoadTimeElapsed(SaveState saveState)
    {
        TimeElapsed = saveState.timeElapsed;
    }
}

