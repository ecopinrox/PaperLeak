public class Checkpoint : Interactible
{
    UIManager uiManager;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UIManager>();
    }

    public override void Interact(out bool uiEnabled)
    {
        uiEnabled = true;

        uiManager.SetCheckpointPanelStatus(true);
    }
}
