using System.Collections;
using UnityEngine;

public class AimingController : MonoBehaviour
{
    [SerializeField] float slowTimeFactor = 0.6f;
    [SerializeField] float maxPositionalError = 0.01f;

    [Header("Target Highlighting")]
    [SerializeField] SpriteRenderer tileHighlight;
    [SerializeField] Color validTargetColor = Color.greenYellow;
    [SerializeField] Color invalidTargetColor = Color.red;

    public static AimingController Instance { get; private set; }

    GridManager gridManager;
    UIManager uiManager;
    PlayerController playerController;

    public enum AimState { Aiming, Finished, Canceled }
    public AimState AimingState { get; private set; } = AimState.Canceled;

    private void Awake()
    {
        Instance = this;

        playerController = FindAnyObjectByType<PlayerController>();
        gridManager = FindAnyObjectByType<GridManager>();
        uiManager = gridManager.GetComponent<UIManager>();
    }

    public async Awaitable<Vector2Int?> Aim(float radius, LayerMask blockingMask)
    {
        //initial setup
        Time.timeScale = slowTimeFactor;
        uiManager.SetAimModePanelStatus(true);
        playerController.SwitchToAimingActionMap();
        tileHighlight.gameObject.SetActive(true);

        AimingState = AimState.Aiming;

        Vector2Int? targetedTilePos = null;

        //main aiming loop
        while (AimingState == AimState.Aiming)
        {
            Vector2Int mousePosition = Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            tileHighlight.transform.position = (Vector2)mousePosition;

            if (IsLocationTargetable(mousePosition, radius, blockingMask))
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
        tileHighlight.gameObject.SetActive(false);

        AimingState = AimState.Finished;
    }

    public void CancelAiming()
    {
        Time.timeScale = 1f;
        uiManager.SetAimModePanelStatus(false);
        tileHighlight.gameObject.SetActive(false);

        AimingState = AimState.Canceled;
    }

    bool IsLocationTargetable(Vector2Int targetedPos, float radius, LayerMask blockingMask)
    {
        Vector2 playerPos = playerController.transform.position;

        bool isWithinRadius = Vector2.Distance(playerPos, targetedPos) <= radius + maxPositionalError;

        RaycastHit2D hit = Physics2D.Raycast(playerPos, targetedPos - playerPos, Vector2.Distance(playerPos, targetedPos), blockingMask);
        //Debug.Log($"{playerPos} -> {targetedPos} [{Vector2.Distance(playerPos, targetedPos)}]: {hit.collider}", hit.collider);
        bool isBlocked = hit;

        return isWithinRadius && !isBlocked;
    }
}
