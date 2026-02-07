using System.Collections;
using UnityEditor;
using UnityEngine;

public class AimingController : MonoBehaviour
{
    [SerializeField] float slowTimeFactor = 0.6f;
    [SerializeField] float maxPositionalError = 0.01f;

    [Header("Target Highlighting")]
    [SerializeField] SpriteRenderer tileHighlight;
    [SerializeField] Color validTargetColor = Color.greenYellow;
    [SerializeField] Color invalidTargetColor = Color.red;

    [Header("Aiming FOV")]
    [SerializeField] AimFOVHandler aimFOVHandler;

    public static AimingController Instance { get; private set; }

    GridManager gridManager;
    UIManager uiManager;
    PlayerController playerController;
    PlayerMovement playerMovement;

    public enum AimState { Aiming, Finished, Canceled }
    public AimState AimingState { get; private set; } = AimState.Canceled;

    private void Awake()
    {
        Instance = this;

        playerController = FindAnyObjectByType<PlayerController>();
        playerMovement = playerController.GetComponent<PlayerMovement>();

        gridManager = FindAnyObjectByType<GridManager>();
        uiManager = gridManager.GetComponent<UIManager>();
    }

    public async Awaitable<Vector2Int?> Aim(float radius, LayerMask targetBlockingMask, LayerMask rayBlockingMask)
    {
        //initial setup
        Time.timeScale = slowTimeFactor;
        uiManager.SetAimModePanelStatus(true);
        playerController.SwitchToAimingActionMap();
        tileHighlight.gameObject.SetActive(true);
        aimFOVHandler.RenderAimFOV(playerMovement.GridBasedPosition, radius, rayBlockingMask);

        AimingState = AimState.Aiming;

        Vector2Int? targetedTilePos = null;

        //main aiming loop
        while (AimingState == AimState.Aiming)
        {
            Vector2Int mousePosition = Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            tileHighlight.transform.position = (Vector2)mousePosition;

            if (IsLocationTargetable(mousePosition, radius, targetBlockingMask, rayBlockingMask))
            {
                tileHighlight.color = validTargetColor;

                targetedTilePos = Vector2Int.RoundToInt(mousePosition);
            }
            else
            {
                tileHighlight.color = invalidTargetColor;

                targetedTilePos = null;
            }

            await Awaitable.NextFrameAsync();
        }

        //check the nature of the conclusion of the loop
        if(AimingState == AimState.Finished)
        {
            return targetedTilePos;
        }
        else
        {
            return null;
        }
    }

    public void FinishAiming()
    {
        Time.timeScale = 1f;
        uiManager.SetAimModePanelStatus(false);
        aimFOVHandler.ClearAimFOV();    
        tileHighlight.gameObject.SetActive(false);

        AimingState = AimState.Finished;
    }

    public void CancelAiming()
    {
        Time.timeScale = 1f;
        uiManager.SetAimModePanelStatus(false);
        aimFOVHandler.ClearAimFOV();    
        tileHighlight.gameObject.SetActive(false);

        AimingState = AimState.Canceled;
    }

    bool IsLocationTargetable(Vector2Int targetedPos, float radius, LayerMask targetBlockingMask, LayerMask rayBlockingMask)
    {
        Vector2 playerPos = playerMovement.GridBasedPosition;

        bool isWithinRadius     = Vector2.Distance(playerPos, targetedPos) <= radius + maxPositionalError;
        bool isTargetBlocked    = gridManager.IsLocationInMask(targetedPos, targetBlockingMask);
        bool isRayBlocked       = Physics2D.Raycast(playerPos, targetedPos - playerPos, Vector2.Distance(playerPos, targetedPos), rayBlockingMask);

        return isWithinRadius && !isTargetBlocked && !isRayBlocked;
    }
}
