using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Tooltip("The build index of the first level. The order of the levels must match the order of the SaveStates in the MasterSave. The class corridor does not count as a level.")]
    [SerializeField] int firstLevelIndex = 1;

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
    int CurrentLevelIndex => CurrentSceneIndex - firstLevelIndex;

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
        //Load the masterSave
        try
        {
            JsonSaver.Load(masterSave, saveFileName);
        }
        catch(FileNotFoundException)
        {
            //If no save file is found (new game), write a new save file with default data
            Debug.LogWarning($"Save file not found at {saveFileName}. Creating new saveFile.");
            JsonSaver.Save(masterSave, saveFileName);
        }

        if(CurrentLevelIndex >= 0)
        {
            _ = DevInitLevel();
        }
    }

    /// <summary>
    /// The actions this function carries out would normally be executed when a level is loaded from the main menu. This function is ideally ONLY used in development, when a scene is loaded directly in play mode.
    /// </summary>
    /// <returns></returns>
    async Awaitable DevInitLevel()
    {
        await Awaitable.NextFrameAsync();

        SaveLevelState();
        LoadDifficultySettings();
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

    //main menu only
    public void StartGame()
    {
        _ = SwitchScene(masterSave.currentLevelIndex + firstLevelIndex);
    }


    //pause menu only
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public async Awaitable SwitchScene(int buildIndex)
    {
        SaveLevelState();

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
        if(CurrentLevelIndex < 0)
        {
            return;
        }

        masterSave.currentLevelIndex = CurrentLevelIndex;

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
        if(CurrentLevelIndex < 0)
        {
            return;
        }

        masterSave.currentLevelIndex = CurrentLevelIndex;

        //if this level has a saved state
        if (masterSave.visited.Contains(CurrentLevelIndex)) 
        {
            //load the saved state
            SaveState saveState = masterSave.GetCurrentLevelState();
            if (saveState != null)
            {
                OnStateSave?.Invoke(saveState);
                JsonSaver.Load(masterSave, saveFileName);

                OnStateLoad?.Invoke(saveState);
            }
            else
            {
                Debug.LogWarning("Load error: No SaveState exists for the current scene.");
            }
        }
        else
        {
            masterSave.visited.Add(CurrentLevelIndex);
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

