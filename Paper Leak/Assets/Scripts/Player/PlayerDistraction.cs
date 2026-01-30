using UnityEngine;

public class PlayerDistraction : Distraction
{
    PlayerMovement playerMovement;
    UIManager uiManager;
    SoundManager soundManager;

    public float ViewDistanceMultiplier { get; private set; } = 1f;

    float walkViewMultiplier;
    float crawlViewMultiplier;

    [SerializeField] SoundData fartSFX;
    [SerializeField] SoundData walkSFX;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        uiManager = FindAnyObjectByType<UIManager>();
        soundManager = uiManager.GetComponent<SoundManager>();
    }

    protected override void OnEnable()
    {
        getPosition = () => playerMovement.GridBasedPosition;
    }

    public void UpdateViewMultiplier()
    {
        ViewDistanceMultiplier = playerMovement.IsCrawling ? crawlViewMultiplier : walkViewMultiplier;
    }

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

        walkViewMultiplier = playerSettings.walkingViewDistanceMultiplier;
        crawlViewMultiplier = playerSettings.crawlingViewDistanceMultiplier;

        UpdateViewMultiplier();
    }
}
