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

    int CurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;

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
    }

    private void OnDisable()
    {
        OnStateLoad -= LoadLandmines;

        OnStateSave -= SaveDifficulty;
        OnStateLoad -= LoadDifficulty;
    }

    private void Start()
    {
        OnStateSave?.Invoke(masterSave.GetCurrentLevelState());

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

            LoadOrInitializeLevelState();
        }
        else if (levelLoadOption == LevelLoadOptions.SaveOnLoad)
        {
            SaveLevelState();
        }
    }

    void LoadDifficultySettings()
    {
        DifficultySwitch.loadDifficultySettings(currentDifficultySetting);
    }

    public async Awaitable LoadLevelFromScratch(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);

        await Awaitable.NextFrameAsync();

        SaveLevelState();
        LoadDifficultySettings();
    }

    public async Awaitable SwitchLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);

        await Awaitable.NextFrameAsync();

        LoadOrInitializeLevelState();
    }

    public async Awaitable ReloadLevelFromScratch() 
    {
        await LoadLevelFromScratch(CurrentSceneIndex);
    }

    public void SaveLevelState()
    {
        masterSave.currentLevelIndex = CurrentSceneIndex;

        SaveState saveState = masterSave.GetCurrentLevelState();
        if (saveState == null)
        { 
            Debug.LogWarning("Save error: No SaveState exists for the current scene.");
            JsonSaver.Save(masterSave, saveFileName);
            return;
        }

        masterSave.visited.Add(masterSave.currentLevelIndex);

        OnStateSave?.Invoke(saveState);
        JsonSaver.Save(masterSave, saveFileName);
    }

    public async Awaitable ReloadLevelState()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(CurrentSceneIndex);

        await Awaitable.NextFrameAsync();

        LoadOrInitializeLevelState();
    }

    public void LoadOrInitializeLevelState()
    {
        masterSave.currentLevelIndex = CurrentSceneIndex;

        //if this level has a saved state
        if (masterSave.visited.Contains(CurrentSceneIndex)) 
        {
            //load the saved state
            SaveState saveState = masterSave.GetCurrentLevelState();
            if (saveState != null)
            {
                OnStateLoad?.Invoke(saveState);
            }
            else
            {
                Debug.LogWarning("Load error: No SaveState exists for the current scene.");
            }
        }
        else
        {
            masterSave.visited.Add(CurrentSceneIndex);
            SaveLevelState();
        }

        LoadDifficultySettings();
    }

    public void SetDifficulty(int difficulty)
    {
        currentDifficultySetting = difficulty;
        LoadDifficultySettings();
    }

    public void DeleteSave()
    {
        try
        {
            JsonSaver.DeleteSaveFile(saveFileName);
            Debug.LogWarning($"Deleted {saveFileName}.");
        }
        catch(DirectoryNotFoundException)
        {
            Debug.Log("Save file not found.");
        }
        catch(IOException)
        {
            Debug.Log("Unable to delete save file as it is currently in use.");
        }
    }

    void LoadLandmines(SaveState saveState)
    {
        foreach (Vector2Int loc in saveState.mineLocations)
        {
            Instantiate(ItemIndexer.GetItem(landmineIndex), (Vector2)loc, Quaternion.identity);
        }
    }

    #region DifficultySavingAndLoading
    void SaveDifficulty(SaveState saveState)
    {
        masterSave.difficulty = currentDifficultySetting;
    }

    void LoadDifficulty(SaveState saveState)
    {
        currentDifficultySetting = masterSave.difficulty;
    }
    #endregion
}

