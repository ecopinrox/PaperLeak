using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField] string sessionStatsFileName;
    [SerializeField] string overallStatsFileName;

    float elapsedTime = 0f;
    public int SaveCount { get; private set; }  = 0;
    bool hasGottenCaught = false;
    bool hasChangedDifficulty = false;

    bool canSaveSessionStats = false;

    public static event Action<float> OnTimeUpdated;
    public static event Action<int> OnSaveCountUpdated;

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
        catch (FileNotFoundException)
        {
            //If no save file is found (new game), write a new save file with default data
            JsonSaver.Save(masterSave, saveFileName);
        }

        if(CurrentSceneIndex >= 0)
        {
            _ = DevInitLevel();
        }

        _ = StatsUpdateLoop(2, destroyCancellationToken);
        _ = UpdateTime(destroyCancellationToken);
    }

    void FixedUpdate()
    {
        OnTimeUpdated?.Invoke(elapsedTime);
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
        canSaveSessionStats = false;
        SceneManager.LoadScene(0);
    }

    public async Awaitable SwitchScene(int buildIndex)
    {
        if(buildIndex < firstLevelIndex)
        {
            canSaveSessionStats = false;
        }

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
                try
                {
                    JsonSaver.Load(masterSave, saveFileName);
                }
                catch(FileNotFoundException)
                {
                    JsonSaver.Save(masterSave, saveFileName);
                }

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
        LoadSessionStats();
        canSaveSessionStats = true;
    }

    public void SetDifficulty(int difficulty)
    {
        currentDifficultySetting = difficulty;
        Instance.RecordDifficultyChanged();
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

        try
        {
            StatsManager.DeleteFile(sessionStatsFileName);
            Debug.LogWarning($"Deleted {sessionStatsFileName}.");
        }
        catch(DirectoryNotFoundException)
        {
            Debug.Log("Session stats file not found.");
        }
        catch(IOException)
        {
            Debug.Log("Unable to delete session stats file as it is currently in use.");
        }
    }

    void LoadLandmines(SaveState saveState)
    {
        foreach (Vector2Int loc in saveState.mineLocations)
        {
            Instantiate(ItemIndexer.GetItem(landmineIndex), (Vector2)loc, Quaternion.identity);
        }
    }

    #region Stats
    async Awaitable StatsUpdateLoop(float period, CancellationToken cancellationToken)
    {
        while(true)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch(OperationCanceledException)
            {
                return;
            }

            if(CurrentLevelIndex < 0 || !canSaveSessionStats || Time.timeScale <= Mathf.Epsilon)
            {
                await Awaitable.FixedUpdateAsync(cancellationToken);
                continue;
            }

            Debug.Log("Save");
            SaveSessionStats();

            if(Time.timeScale > Mathf.Epsilon)
            {
                await Task.Delay((int)(1000 * period));
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }
    }

    void SaveSessionStats()
    {
        StatsManager.WriteStats(sessionStatsFileName, new(
            elapsedTime,
            SaveCount,
            !hasGottenCaught,
            !hasChangedDifficulty
        ));
    }

    void LoadSessionStats()
    {
        Debug.Log("loaded");
        GameStats stats = StatsManager.ReadStats(sessionStatsFileName);
        if(stats != null)
        {
            elapsedTime = stats.timeToBeat;
            SetSaveCount(stats.saveCount);
            hasGottenCaught = !stats.clearedWithoutGettingCaught;
            hasChangedDifficulty = !stats.clearedWithoutChangingDifficulty;
        }
    }

    async Awaitable UpdateTime(CancellationToken cancellationToken)
    {
        while(true)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch(OperationCanceledException)
            {
                return;
            }

            await Awaitable.NextFrameAsync(cancellationToken);

            if(CurrentLevelIndex >= 0 && Time.timeScale > Mathf.Epsilon)
            {
                elapsedTime += Time.unscaledDeltaTime;
            }
        }
    }

    void SetSaveCount(int saveCount)
    {
        SaveCount = saveCount;
        OnSaveCountUpdated?.Invoke(SaveCount);
    }

    public void IncrementSaveCount()
    {
        SaveCount++;
        OnSaveCountUpdated?.Invoke(SaveCount);
    }

    public void RecordPlayerCaught()
    {
        hasGottenCaught = true;
    }

    public void RecordDifficultyChanged()
    {
        hasChangedDifficulty = true;
    }
        
    public bool CheckForOverallStats()
    {
        return StatsManager.CheckForStats(overallStatsFileName);
    }

    public void DeleteOverallStats()
    {
        try
        {
            StatsManager.DeleteFile(overallStatsFileName);
            Debug.LogWarning($"Deleted {overallStatsFileName}.");
        }
        catch(DirectoryNotFoundException)
        {
            Debug.Log("Overall stats file not found.");
        }
        catch(IOException)
        {
            Debug.Log("Unable to delete overall stats file as it is currently in use.");
        }
    }
    #endregion

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

