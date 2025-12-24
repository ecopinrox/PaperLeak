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
    [SerializeField] GameObject checkpointPanel;
    [SerializeField] GameObject changeDifficultyPanel;
    [SerializeField] Button[] difficultyButtons;

    [Header("Items")]
    [SerializeField] List<ItemSlotUIElement> itemSlots;

    [Header("Collectibles")]
    [SerializeField] List<GameObject> collectibleIcons;

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Difficulty")]
    [SerializeField] TextMeshProUGUI difficultyText;

    PlayerController playerController;

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void OnEnable()
    {
        DifficultySwitch.loadDifficultySettings += SetDifficultyText;
    }

    private void OnDisable()
    {
        DifficultySwitch.loadDifficultySettings -= SetDifficultyText;
    }

    private void Start()
    {
        foreach(GameObject obj in collectibleIcons)
        {
            obj.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        timerText.text = TimeSpan.FromSeconds(LevelManager.Instance.TimeElapsed).ToString(@"hh\:mm\:ss");  
    }

    public void SetGameOverPanelStatus(bool active) => gameOverPanel.SetActive(active);

    public void SetAimModePanelStatus(bool active) => aimModeFilterPanel.SetActive(active); 

    public void SetCheckpointPanelStatus(bool active) 
    {
        playerController.EnterUIActionMap();
        checkpointPanel.SetActive(active); 
    }

    public void SetChangeDifficultyPanelStatus(bool active)
    {
        changeDifficultyPanel.SetActive(active);
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

    public void UpdateItemSlot(GameObject itemPrefab, int count, int slotIndex)
    {
        Debug.Log("updating " + slotIndex);
        itemSlots[slotIndex].UpdateSlot(itemPrefab, count);
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

    public void DisableAllMenus()
    {
        playerController.ExitUIActionMap();
        checkpointPanel.SetActive(false);
    }
}
