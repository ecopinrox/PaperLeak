public class Checkpoint : Interactible
{
    UIManager uiManager;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UIManager>();
    }

    public override bool Interact()
    {
        uiManager.SetCheckpointPanelStatus(true);
        return true;
    }
}
