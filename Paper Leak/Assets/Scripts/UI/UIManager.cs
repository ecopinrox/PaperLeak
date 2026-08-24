using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject aimModeFilterPanel;

    [Header("Menus")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject checkpointPanel;
    [SerializeField] GameObject changeDifficultyPanel;
    [SerializeField] Button[] difficultyButtons;

    [Header("Dialogue")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Image portraitImage;
    [SerializeField] TextMeshProUGUI speakerName;
    [SerializeField] TextMeshProUGUI dialogueText;

    [Header("Interaction")]
    [SerializeField] GameObject interactionPromptPanel;

    [Header("Items")]
    [SerializeField] List<ItemSlotUIElement> itemSlots;

    [Header("Collectibles")]
    [SerializeField] List<GameObject> collectibleIcons;

    [Header("Difficulty")]
    [SerializeField] TextMeshProUGUI difficultyText;

    [Header("Volume Sliders")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Stats")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI saveCountText;

    [Header("Completion Menu")]
    [SerializeField] GameObject completionPanel;
    [SerializeField] TextMeshProUGUI timerTextValue;
    [SerializeField] TextMeshProUGUI saveCountTextValue;
    [SerializeField] TextMeshProUGUI caughtTextValue;
    [SerializeField] TextMeshProUGUI changedDifficultyTextValue;

    PlayerController playerController;

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();

        SetDialoguePanelStatus(false);
    }

    private void OnEnable()
    {
        DifficultySwitch.loadDifficultySettings += SetDifficultyText;

        PlayerInteractionHandler.OnInteractibleFound += EnableInteractionPromptPanel;
        PlayerInteractionHandler.OnInteractibleCleared += DisableInteractionPromptPanel;

        LevelManager.OnTimeUpdated += UpdateTimerText;
        LevelManager.OnSaveCountUpdated += UpdateSaveCountText;
    }

    private void OnDisable()
    {
        DifficultySwitch.loadDifficultySettings -= SetDifficultyText;

        PlayerInteractionHandler.OnInteractibleFound -= EnableInteractionPromptPanel;
        PlayerInteractionHandler.OnInteractibleCleared -= DisableInteractionPromptPanel;

        LevelManager.OnTimeUpdated -= UpdateTimerText;
        LevelManager.OnSaveCountUpdated -= UpdateSaveCountText;
    }

    private void Start()
    {
        musicSlider.value = FindAnyObjectByType<MusicManager>().GetMusicVolume();
        sfxSlider.value = FindAnyObjectByType<SoundManager>().GetSFXVolume();

        UpdateSaveCountText(LevelManager.Instance.SaveCount);
        if(!LevelManager.Instance.CheckForOverallStats())
        {
            if(timerText != null) timerText.gameObject.SetActive(false);
            if(saveCountText != null) saveCountText.gameObject.SetActive(false);
        }
    }

    public void SetGameOverPanelStatus(bool active) => gameOverPanel.SetActive(active);

    public void SetAimModePanelStatus(bool active) => aimModeFilterPanel.SetActive(active); 

    public void SetCheckpointPanelStatus(bool active) 
    {
        playerController.SwitchToUIActionMap();
        checkpointPanel.SetActive(active); 
    }

    public void SetPausePanelStatus(bool active)
    {
        pausePanel.SetActive(active);
    }

    public void SetChangeDifficultyPanelStatus(bool active)
    {
        changeDifficultyPanel.SetActive(active);
    }

    public void SetDialoguePanelStatus(bool active)
    {
        dialoguePanel.SetActive(active);
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        portraitImage.sprite = dialogue.portrait;
        speakerName.text = dialogue.name;   
        dialogueText.text = dialogue.text;

        if(dialogue.iconIndex is int iconIndex)
        {
            collectibleIcons[iconIndex].SetActive(false);
        }
    }

    void EnableInteractionPromptPanel()
    {
        interactionPromptPanel.SetActive(true);
    }

    void DisableInteractionPromptPanel()
    {
        interactionPromptPanel.SetActive(false);
    }

    public void SetActiveDifficultyButtons()
    {
        for(int i = 0; i < difficultyButtons.Length; i++)
        {
            if(i == LevelManager.currentDifficultySetting)
            {
                difficultyButtons[i].interactable = false;
            }
            else
            {
                difficultyButtons[i].interactable = true;
            }
        }
    }

    public void SetDifficultyText()
    {
        difficultyText.text = LevelManager.currentDifficultySetting switch
        {
            0 => "Easy",
            1 => "Normal",
            2 => "Hard",
            _ => "<?>"
        };
    }

    public void SetDifficultyText(int difficulty)
    {
        SetDifficultyText();
    }

    public void UpdateItemSlot(GameObject itemPrefab, int count, bool isInfinite, int slotIndex)
    {
        itemSlots[slotIndex].UpdateSlot(itemPrefab, count, isInfinite);
    }

    public void UpdateCooldownOverlay(float fill, int slotIndex)
    {
        itemSlots[slotIndex].UpdateCooldownOverlay(fill);
    }

    public void SelectItemSlot(int slotIndex)
    {
        foreach(ItemSlotUIElement slot in itemSlots)
        {
            slot.DeselectSlot();
        }
        itemSlots[slotIndex].SelectSlot();
    }

    public void UpdateCollectibleIcons(HashSet<int> collectibleIDs)
    {
        for (int i = 0; i < collectibleIcons.Count; i++)
        {
            if (collectibleIDs.Contains(i)) collectibleIcons[i].SetActive(true);
            else collectibleIcons[i].SetActive(false);
        }
    }

    public void UpdateTimerText(float time)
    {
        if(timerText == null)
        {
            return;
        }

        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timerText.text = timeSpan.ToString(@"hh\:mm\:ss");
    }

    public void UpdateSaveCountText(int saves)
    {
        if(saveCountText == null)
        {
            return;
        }

        saveCountText.text = $"Saves: {saves}";
    }

    public void DisableAllMenus()
    {
        playerController.SwitchToPlayerActionMap();
        checkpointPanel.SetActive(false);
    }

    public void EnableCompletionScreen(GameStats stats)
    {
        completionPanel.SetActive(true);
        timerTextValue.text = TimeSpan.FromSeconds(stats.timeToBeat).ToString(@"hh\:mm\:ss");
        saveCountTextValue.text = stats.saveCount.ToString();
        caughtTextValue.text = stats.clearedWithoutGettingCaught ? "No" : "Yes";
        changedDifficultyTextValue.text = stats.clearedWithoutChangingDifficulty ? "No" : "Yes";
    }
}
