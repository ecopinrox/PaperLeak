using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    PlayerInput playerInput;
    PlayerMovement playerMovement;
    PlayerInventory playerInventory;
    SpriteRenderer spriteRenderer;
    PlayerDistraction playerDistraction;
    PlayerCameraRigHandler playerCameraRigHandler;
    UIManager uiManager;
    AimingController aimingController;

    [SerializeField] DifficultySwitch playerDifficultySwitch;
    
    InputAction moveAction;
    InputAction crawlAction;
    InputAction peekAction;
    InputAction interactAction;
    InputAction exitUIAction;
    InputAction useItemAction;
    InputAction selectItem1Action;
    InputAction selectItem2Action;
    InputAction selectItem3Action;
    InputAction selectItem4Action;

    InputAction selectTargetAction;
    InputAction stopAimingAction;

    const string movementActionMapName = "Player";
    const string aimingActionMapName = "Aiming";
    const string uiActionMapName = "UI";

    Interactible interactible;

    void Awake()
    {
        Instance = this;

        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInventory = GetComponent<PlayerInventory>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerDistraction = GetComponent<PlayerDistraction>();
        playerCameraRigHandler = GetComponent<PlayerCameraRigHandler>();
        uiManager = FindFirstObjectByType<UIManager>();
        aimingController = uiManager.GetComponent<AimingController>();

        moveAction              = playerInput.actions["Move"            ];
        crawlAction             = playerInput.actions["Crawl"           ];
        peekAction              = playerInput.actions["Peek"            ];
        interactAction          = playerInput.actions["Interact"        ];
        exitUIAction            = playerInput.actions["Exit"            ];
        useItemAction           = playerInput.actions["UseItem"         ];
        selectItem1Action       = playerInput.actions["SelectItem1"     ];
        selectItem2Action       = playerInput.actions["SelectItem2"     ];
        selectItem3Action       = playerInput.actions["SelectItem3"     ];
        selectItem4Action       = playerInput.actions["SelectItem4"     ];

        selectTargetAction      = playerInput.actions["SelectTarget"    ];
        stopAimingAction        = playerInput.actions["StopAiming"      ];
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

        peekAction.performed += SetPeekingCamera;

        crawlAction.performed += ToggleCrawl;

        interactAction.performed += Interact;

        exitUIAction.performed += ExitUI;

        useItemAction.performed += UseItem;

        selectItem1Action.performed += SelectItem1;
        selectItem2Action.performed += SelectItem2;
        selectItem3Action.performed += SelectItem3;
        selectItem4Action.performed += SelectItem4;

        selectTargetAction.performed += SelectTarget;
        stopAimingAction.performed += StopAiming;

        DifficultySwitch.loadDifficultySettings += LoadDifficulty;

        LevelManager.OnStateLoad += LoadPosision;
        LevelManager.OnStateSave += SavePosition;

        StartCoroutine(playerMovement.MovementHandler());
    }

    private void OnDisable()
    {
        moveAction.started -= SetDirection;
        moveAction.performed -= SetDirection;
        moveAction.canceled -= SetDirection;

        peekAction.performed -= SetPeekingCamera;

        crawlAction.performed -= ToggleCrawl;

        interactAction.performed -= Interact;

        exitUIAction.performed -= ExitUI;

        useItemAction.performed -= UseItem;

        selectItem1Action.performed -= SelectItem1;
        selectItem2Action.performed -= SelectItem2;
        selectItem3Action.performed -= SelectItem3;
        selectItem4Action.performed -= SelectItem4;

        selectTargetAction.performed -= SelectTarget;
        stopAimingAction.performed -= StopAiming;

        LevelManager.OnStateLoad -= LoadPosision;
        LevelManager.OnStateSave -= SavePosition;

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

    public void SwitchToAimingActionMap()
    {
        SwitchActionMap(aimingActionMapName);
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

    void SavePosition(SaveState saveState)
    {
        saveState.playerPos = playerMovement.GridBasedPosition;
    }

    void LoadPosision(SaveState saveState)
    {
        transform.position = (Vector2)saveState.playerPos;
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
        playerCameraRigHandler.SetActiveCamera(Vector2.zero, false);
        playerDistraction.UpdateViewMultiplier();
    }

    void SetDirection(InputAction.CallbackContext ctx)
    {
        Vector2 direction = ctx.ReadValue<Vector2>();

        playerMovement.SetDirection(direction);
        SetMovementCamera(direction);
    }

    void SetMovementCamera(Vector2 direction)
    {
        playerCameraRigHandler.SetActiveCamera(direction, false);
    }

    void SetPeekingCamera(InputAction.CallbackContext ctx) 
    { 
        Vector2 direction = ctx.ReadValue<Vector2>();
        if (direction == Vector2.zero) return;

        playerCameraRigHandler.SetActiveCamera(direction, true);
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

    void ExitUI(InputAction.CallbackContext ctx)
    {
        uiManager.DisableAllMenus();
        SwitchActionMap(movementActionMapName);
    }

    async void UseItem(InputAction.CallbackContext _)
    {
        await playerInventory.UseSelectedItem();
    }

    void SelectItem1(InputAction.CallbackContext _)
    {
        playerInventory.SelectItemSlot(0);
    }

    void SelectItem2(InputAction.CallbackContext _)
    {
        playerInventory.SelectItemSlot(1);
    }

    void SelectItem3(InputAction.CallbackContext _)
    {
        playerInventory.SelectItemSlot(2);
    }

    void SelectItem4(InputAction.CallbackContext _)
    {
        playerInventory.SelectItemSlot(3);
    }

    void SelectTarget(InputAction.CallbackContext _)
    {
        aimingController.FinishAiming();
        SwitchActionMap(movementActionMapName);
    }

    void StopAiming(InputAction.CallbackContext _)
    {
        aimingController.CancelAiming();
        SwitchActionMap(movementActionMapName);
    }

    #endregion
}
