using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{ 
    GridManager gridMovementMonitor;
    PlayerDistraction playerDistraction;
    PlayerAnimation playerAnimation;

    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float crawlSpeed = 2f;
    [SerializeField] float walkSFXDelay = 0.3f;

    public bool IsCrawling { get; private set; }
    public Vector2Int GridBasedPosition { get; private set; }

    public enum Direction { None, Up,  Down, Left, Right };
    public Direction CurrentDirection { get; private set; } = Direction.None;

    bool shouldStopMoving = false;

    void Awake()
    {
        gridMovementMonitor = FindFirstObjectByType<GridManager>();
        playerDistraction = GetComponent<PlayerDistraction>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void OnEnable()
    {
        GridBasedPosition = Vector2Int.RoundToInt(transform.position);
    }

    public void SetDirection(Vector2 input)
    {
        if (input.x > 0) CurrentDirection = Direction.Right;
        else if (input.x < 0) CurrentDirection = Direction.Left;
        else if (input.y > 0) CurrentDirection = Direction.Up;
        else if (input.y < 0) CurrentDirection = Direction.Down;
        else CurrentDirection = Direction.None;
    }

    public IEnumerator MovementHandler()
    {
        gridMovementMonitor.BlockTile(GridBasedPosition);

        while (true)
        {
            yield return new WaitForEndOfFrame();

            Vector2Int displacement = CurrentDirection switch
            {
                Direction.Left  => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                Direction.Up    => Vector2Int.up,
                Direction.Down  => Vector2Int.down,
                _               => Vector2Int.zero
            };

            playerAnimation.SetDirection(displacement);

            Vector2Int nextPosition = Vector2Int.RoundToInt(transform.position) + displacement;
            if (!gridMovementMonitor.CanMove(nextPosition, IsCrawling))
            {
                playerAnimation.SetMoving(false);
                continue;
            }

            playerAnimation.SetMoving(true);
            Vector2Int prevPosition = GridBasedPosition;
            GridBasedPosition = nextPosition;
            gridMovementMonitor.BlockTile(GridBasedPosition);

            yield return StartCoroutine(WalkCoroutine(GridBasedPosition));

            gridMovementMonitor.UnblockTile(prevPosition);
        }
    }

    IEnumerator WalkCoroutine(Vector2 destination)
    {
        float delay = walkSFXDelay;

        while (transform.position != (Vector3)destination)
        {
            float speed = IsCrawling ? crawlSpeed : moveSpeed;
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime);

            if (delay >= walkSFXDelay && !IsCrawling)
            {
                playerDistraction.PlayWalkSFX();
                delay = 0;
            }
            else
            {
                delay += Time.fixedDeltaTime;
            }

            if(shouldStopMoving)
            {
                shouldStopMoving = false;
                transform.position = (Vector2)Vector2Int.RoundToInt(transform.position);
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    public void SnapToPosition(Vector2Int pos)
    {
        shouldStopMoving = true;
        transform.position = (Vector2)pos;
        GridBasedPosition = pos;
    }

    public void ToggleProne()
    {
        if (gridMovementMonitor.IsLocationInMask(GridBasedPosition, LayerMask.GetMask("Crawlable"))) return;
        IsCrawling = !IsCrawling;
        playerAnimation.SetCrouching(IsCrawling);
    }
}