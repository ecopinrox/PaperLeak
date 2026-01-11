using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{ 
    GridManager gridMovementMonitor;
    PlayerDistraction playerDistraction;

    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float crawlSpeed = 2f;
    [SerializeField] float walkSFXDelay = 0.3f;

    public bool IsCrawling { get; private set; }
    public Vector2Int GridBasedPosition {  get; private set; }

    public enum Direction { None, Up,  Down, Left, Right };
    public Direction CurrentDirection { get; private set; } = Direction.None;

    void Awake()
    {
        gridMovementMonitor = FindFirstObjectByType<GridManager>();
        playerDistraction = GetComponent<PlayerDistraction>();
    }

    private void OnEnable()
    {
        LevelManager.OnLoadState += LoadPosition;
    }

    private void Start()
    {
        GridBasedPosition = Vector2Int.RoundToInt(transform.position);
    }

    private void OnDisable()
    {
        LevelManager.OnLoadState -= LoadPosition;
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

            if (!gridMovementMonitor.CanMove(transform.position, displacement, IsCrawling)) continue;
            GridBasedPosition = Vector2Int.RoundToInt(transform.position) + displacement;

            yield return StartCoroutine(WalkCoroutine((Vector2)transform.position + displacement));
        }
    }

    IEnumerator WalkCoroutine(Vector2 destination)
    {
        LookAt(destination - (Vector2)transform.position);
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
            else delay += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    public void ToggleProne()
    {
        if (gridMovementMonitor.IsLocationInMask(GridBasedPosition, LayerMask.GetMask("Crawlable"))) return;
        IsCrawling = !IsCrawling;
    }

    void LookAt(Vector2 direction)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    void LoadPosition()
    {
        transform.position = (Vector2)LevelManager.SaveState.playerPos;
    }
}