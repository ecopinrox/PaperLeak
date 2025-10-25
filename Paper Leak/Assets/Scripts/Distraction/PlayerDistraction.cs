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

    [SerializeField] GameObject paperBall;
    [SerializeField] GameObject popPop;
    [HideInInspector] public bool isPopPop;

    [SerializeField] int paperBallIncrementCount = 3;

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

    public bool Throw(bool isCrawling)
    {
        if (aimingController.SelectedPos == null) { return false; }

        Vector2Int pos = (Vector2Int)aimingController.SelectedPos;
        if (!isPopPop && playerInventory.PaperBallCount > 0)
        {
            GameObject instance = Instantiate(paperBall);
            instance.transform.position = transform.position;

            instance.GetComponent<PaperBallController>().SetDestination(pos, isCrawling);
            playerInventory.DecrementPaperBallCount();
        }
        else if (isPopPop && playerInventory.PopPopCount > 0)
        {
            GameObject instance = Instantiate(popPop);
            instance.transform.position = transform.position;

            instance.GetComponent<PopPopController>().SetDestination(pos, isCrawling);
            playerInventory.DecrementPopPopCount();
        }

        UpdateDistractionInformation();
        ExitAimMode();

        return true;
    }
    
    public void TearPaper()
    {
        soundManager.PlaySound(paperTearingSFX, Position, this);
        playerInventory.IncrementPaperBallCount(paperBallIncrementCount);
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
        paperTearingSFX = playerSettings.paperTearingSFX;
    }

    public void UpdateDistractionInformation() => uiManager.UpdateDistractionInformation(isPopPop, playerInventory.PaperBallCount, playerInventory.PopPopCount);
}
