using UnityEngine;

public class PlayerDistraction : Distraction
{
    [SerializeField] float crawlViewMultiplier = 0.6f;

    PlayerMovement playerMovement;
    UIManager uiManager;
    SoundManager soundManager;

    public float ViewDistanceMultiplier { get; private set; } = 1f;

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

    public void UpdateViewMultiplier(bool isCrouched)
    {
        ViewDistanceMultiplier = isCrouched ? crawlViewMultiplier : 1f;
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
        Debug.Log("loaded " + playerSettings.walkSFX.name + ", formerly " + walkSFX.name);
        walkSFX = playerSettings.walkSFX;
    }
}
