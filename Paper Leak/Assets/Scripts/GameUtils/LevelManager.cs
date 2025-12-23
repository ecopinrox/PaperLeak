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

    public static event Action OnLoadState;
    public static bool isReloading = false;

    public static int currentDifficultySetting = 1;

    public HashSet<Vector2Int> collectedItemLocations = new();
    public HashSet<Vector2Int> openedDoors = new();

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
    }

    private void OnEnable()
    {
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
        isReloading = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        StartCoroutine(LoadAfterDelay());
    }

    public void LoadLevel(string levelName)
    {
        saveState.collectibles = new();
        saveState.openedDoors = new();
        collectedItemLocations = new();

        isReloading = false;
        TimeElapsed = 0;
        SceneManager.LoadScene(levelName);

        //Clear save after level loads
        StartCoroutine(ExecuteAfterDelay(Save));
        StartCoroutine(ExecuteAfterDelay(LoadDifficultySettings));
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
        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();

        saveState.playerPos = Vector2Int.RoundToInt(playerInventory.transform.position);
        saveState.openedDoors = new(openedDoors);
        saveState.collectibles = new(playerInventory.Collectibles);
        saveState.collectedItemLocations = new(collectedItemLocations);
        saveState.timeElapsed = TimeElapsed;
    }

    public void SetDifficulty(int difficulty)
    {
        currentDifficultySetting = difficulty;
        LoadDifficultySettings();
    }

    public void RegisterOpenDoor(Vector2Int doorPos)
    {
        openedDoors.Add(doorPos);
    }

    IEnumerator LoadAfterDelay()
    {
        yield return null;
        //if (newLevel)
        //{
        //    collectedItemLocations = new();
        //}
        collectedItemLocations = new(saveState.collectedItemLocations);
        TimeElapsed = saveState.timeElapsed;
        OnLoadState?.Invoke();
        LoadDifficultySettings();
    }
}
