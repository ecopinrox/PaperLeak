using UnityEngine;

public class PlayerDistraction : Distraction
{
    [SerializeField] float crawlViewMultiplier = 0.6f;

    PlayerInventory playerInventory;
    PlayerMovement playerMovement;
    UIManager uiManager;
    AimingController aimingController;
    SoundManager soundManager;

    public float ViewDistanceMultiplier { get; private set; } = 1f;

    [SerializeField] SoundData fartSFX;
    SoundData walkSFX;
    SoundData paperTearingSFX;

    void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerMovement = GetComponent<PlayerMovement>();

        uiManager = FindAnyObjectByType<UIManager>();
        aimingController = uiManager.GetComponent<AimingController>();
        soundManager = uiManager.GetComponent<SoundManager>();
    }

    protected override void OnEnable()
    {
        getPosition = () => playerMovement.GridBasedPosition;
    }

    public void UpdateViewMultiplier(bool isCrouched)
    {
        ViewDistanceMultiplier = isCrouched ? crawlViewMultiplier : 1f;
    }

    public void EnterAimMode() => aimingController.EnterAimMode();

    public void ExitAimMode() => aimingController.ExitAimMode();

    public void Fart()
    {
        soundManager.PlaySound(fartSFX, Position, this);
    }

    public void PlayWalkSFX()
    {
        soundManager.PlaySound(walkSFX, Position, this);
    }

    public void LoadSettings(PlayerSettings playerSettings)
    {
        walkSFX = playerSettings.walkSFX;
        paperTearingSFX = playerSettings.paperTearingSFX;
    }
}
