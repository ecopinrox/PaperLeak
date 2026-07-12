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

    [Header("Items")]
    [SerializeField] List<ItemSlotUIElement> itemSlots;

    [Header("Collectibles")]
    [SerializeField] List<GameObject> collectibleIcons;

    [Header("Difficulty")]
    [SerializeField] TextMeshProUGUI difficultyText;

    PlayerController playerController;

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();

        SetDialoguePanelStatus(false);
    }

    private void OnEnable()
    {
        DifficultySwitch.loadDifficultySettings += SetDifficultyText;
    }

    private void OnDisable()
    {
        DifficultySwitch.loadDifficultySettings -= SetDifficultyText;
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

    public void DisableAllMenus()
    {
        playerController.SwitchToPlayerActionMap();
        checkpointPanel.SetActive(false);
    }
}
