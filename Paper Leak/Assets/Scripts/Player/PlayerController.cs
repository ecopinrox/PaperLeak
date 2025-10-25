using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerMovement playerMovement;
    SpriteRenderer spriteRenderer;
    PlayerDistraction playerDistraction;
    PlayerCameraRigHandler playerCameraRigHandler;
    UIManager uiManager;

    [SerializeField] DifficultySwitch playerDifficultySwitch;
    
    InputAction moveAction;
    InputAction crawlAction;
    InputAction peekAction;
    InputAction aimAction;
    InputAction interactAction;
    InputAction throwAction;
    InputAction cancelAction;
    InputAction selectVDAction;
    InputAction selectSDAction;
    InputAction makePaperBallsAction;
    InputAction fartAction;
    InputAction exitUIAction;

    const string movementActionMapName = "Player";
    const string aimingActionMapName = "Aiming";
    const string uiActionMapName = "UI";

    Interactible interactible;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerDistraction = GetComponent<PlayerDistraction>();
        playerCameraRigHandler = GetComponent<PlayerCameraRigHandler>();
        uiManager = FindFirstObjectByType<UIManager>();
        //CameraSwitcher = FindFirstObjectByType<PlayerCameraHandler>();

        moveAction              = playerInput.actions["Move"                    ];
        crawlAction             = playerInput.actions["Crawl"                   ];
        peekAction              = playerInput.actions["Peek"                    ];
        aimAction               = playerInput.actions["Aim"                     ];
        interactAction          = playerInput.actions["Interact"                ];
        throwAction             = playerInput.actions["Throw"                   ];
        cancelAction            = playerInput.actions["Cancel"                  ];
        selectVDAction          = playerInput.actions["SelectVisualDistraction" ];
        selectSDAction          = playerInput.actions["SelectSoundDistraction"  ];
        makePaperBallsAction    = playerInput.actions["MakePaperBalls"          ];
        fartAction              = playerInput.actions["Fart"                    ];
        exitUIAction            = playerInput.actions["Exit"                    ];
    }

    //moving also counts as peeking for some odd reason but the vector read is (0,0) so it shouldn't(?) matter for my purposes
    void OnEnable()
    {
        foreach(InputActionMap actionMap in playerInput.actions.actionMaps) 
        {
            actionMap.Disable();
        }
        SwitchActionMap(movementActionMapName);

        moveAction.started += SetDirection;
        moveAction.performed += SetDirection;
        moveAction.canceled += SetDirection;

        peekAction.performed += SetCameras;

        crawlAction.performed += ToggleCrawl;

        aimAction.performed += StartAiming;

        cancelAction.started += StopAiming;

        interactAction.performed += Interact;

        throwAction.performed += Throw;

        selectVDAction.performed += SelectVisualDistraction;

        selectSDAction.performed += SelectSoundDistraction;

        makePaperBallsAction.performed += MakePaperBalls;

        fartAction.performed += Fart;

        exitUIAction.performed += ExitUI;

        DifficultySwitch.loadDifficultySettings += LoadDifficulty;

        StartCoroutine(playerMovement.MovementHandler());
    }

    private void OnDisable()
    {
        moveAction.started -= SetDirection;
        moveAction.performed -= SetDirection;
        moveAction.canceled -= SetDirection;

        peekAction.performed -= SetCameras;

        crawlAction.performed -= ToggleCrawl;

        aimAction.performed -= StartAiming;

        cancelAction.started -= StopAiming;
        
        interactAction.performed -= Interact;

        throwAction.performed -= Throw;

        selectVDAction.performed -= SelectVisualDistraction;

        selectSDAction.performed -= SelectSoundDistraction;

        makePaperBallsAction.performed -= MakePaperBalls;

        fartAction.performed -= Fart;

        exitUIAction.performed -= ExitUI;

        DifficultySwitch.loadDifficultySettings -= LoadDifficulty;
    }

    public void SetInteractible(Interactible interactible)
    {
        this.interactible = interactible;
    }

    public void ClearInteractible()
    {
        interactible = null;
    }

    public void EnterUIActionMap()
    {
        SwitchActionMap(uiActionMapName);
    }

    public void ExitUIActionMap()
    {
        SwitchActionMap(movementActionMapName);
    }

    void SwitchActionMap(string actionMapName)
    {
        foreach(InputActionMap actionMap in playerInput.actions.actionMaps) 
        {
            actionMap.Disable();
        }
        playerInput.SwitchCurrentActionMap(actionMapName);
    }

    void LoadDifficulty(int level)
    {
        PlayerSettings settings = playerDifficultySwitch.GetDifficultySettings<PlayerSettings>(level);

        playerDistraction.LoadSettings(settings);
    }

    #region InputMethods
    /// <summary>
    /// temporary function, change when i get the actual sprites
    /// </summary>
    void ToggleCrawl(InputAction.CallbackContext ctx)
    {
        const int playerProneLayerIndex = 7;
        const int playerLayerIndex = 6;

        playerMovement.ToggleProne();
        if (playerMovement.IsCrawling)
        {
            spriteRenderer.color = Color.red;
            gameObject.layer = playerProneLayerIndex;
        }
        else
        {
            if (spriteRenderer != null) spriteRenderer.color = Color.blue;
            gameObject.layer = playerLayerIndex;
        }

        //CameraSwitcher.SetCamera(Vector2.zero);
        playerCameraRigHandler.SetActiveCamera(Vector2.zero);
        playerDistraction.UpdateViewMultiplier(playerMovement.IsCrawling);
    }

    void SetDirection(InputAction.CallbackContext ctx) => playerMovement.SetDirection(ctx.ReadValue<Vector2>());

    void SetCameras(InputAction.CallbackContext ctx) 
    { 
        //CameraSwitcher.SetCamera(ctx.ReadValue<Vector2>()); 
        playerCameraRigHandler.SetActiveCamera(ctx.ReadValue<Vector2>());
    }

    void StartAiming(InputAction.CallbackContext ctx)
    {
        SwitchActionMap(aimingActionMapName);
        playerDistraction.EnterAimMode();
    }

    void StopAiming(InputAction.CallbackContext ctx)
    {
        SwitchActionMap(movementActionMapName);
        playerDistraction.ExitAimMode();
        //CameraSwitcher.SetCamera(Vector2.zero);
        playerCameraRigHandler.SetActiveCamera(ctx.ReadValue<Vector2>());
    }

    void Interact(InputAction.CallbackContext ctx) 
    {
        if (!interactible)
        {
            return;
        }

        interactible.Interact(out bool uiEnabled);
        if(uiEnabled) SwitchActionMap(uiActionMapName);
    }

    void Throw(InputAction.CallbackContext ctx)
    {
        if (playerDistraction.Throw(playerMovement.IsCrawling))
        {
            SwitchActionMap(movementActionMapName);
        }
    }

    void SelectVisualDistraction(InputAction.CallbackContext ctx)
    {
        playerDistraction.isPopPop = false;
        playerDistraction.UpdateDistractionInformation();
    }

    void SelectSoundDistraction(InputAction.CallbackContext ctx)
    {
        playerDistraction.isPopPop = true;
        playerDistraction.UpdateDistractionInformation();
    }

    void MakePaperBalls(InputAction.CallbackContext ctx)
    {
        playerDistraction.TearPaper();
    } 
    
    private void Fart(InputAction.CallbackContext context)
    {
        playerDistraction.Fart();
    }

    void ExitUI(InputAction.CallbackContext ctx)
    {
        uiManager.DisableAllMenus();
        SwitchActionMap(movementActionMapName);
    }
    #endregion
}
