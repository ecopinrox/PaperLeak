using UnityEngine;

public class Checkpoint : Interactible
{
    UIManager uiManager;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y);
    }

    public override void Interact()
    {
        uiManager.SetCheckpointPanelStatus(true);
    }
}
