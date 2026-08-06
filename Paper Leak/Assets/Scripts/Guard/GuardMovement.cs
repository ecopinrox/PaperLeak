using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class GuardMovement : MonoBehaviour
{
    //manages guard movement logic
    //moves from 1 tile to another

    [Header("Movement")]
    float patrolSpeed = 3f;
    float investigationSpeed = 4f;
    [SerializeField] float maxPositionalError = 0.01f;
    [SerializeField] LayerMask excludedLayersInPathfinding;

    /// <summary>
    /// The maximum tile that the guard waits for a tile to be unblocked before moving to it anyway. This prevents deadlocks.
    /// </summary>
    [SerializeField] float maxTileBlockWaitSeconds = 4f;

    float speed;

    /// <summary>
    /// Holds the location of the tile that the guard is at/currently travelling to.
    /// </summary>
    public Vector2Int GridBasedPosition { get; private set; }

    List<Node> path = new();
    IEnumerator<Node> pathIterator;
    public bool PathComplete { get; private set; } = true;
    bool shouldStopMoving = false;
    public bool IsCrouching { get; private set; }

    public Vector2 FacingDir { get; private set; } = Vector2.up;

    Pathfinder pathfinder;
    GridManager gridManager;

    GuardAnimation guardAnimation;

    void Awake()
    {
        pathfinder = FindFirstObjectByType<Pathfinder>();
        gridManager = pathfinder.GetComponent<GridManager>();

        guardAnimation = GetComponent<GuardAnimation>();
    }

    void Start()
    {
        SetSpeed(false);
        GridBasedPosition = Vector2Int.RoundToInt(transform.position);

        StartCoroutine(GuardMovementHandler());
    }

    public void SetSpeed(bool investigating)
    {
        speed = investigating ? investigationSpeed : patrolSpeed;
    }

    public void SetDestination(Vector2Int loc)
    {
        if (path != null && path.Count > 0 && path[^1].pos == loc) return;

        List<Node> newPath = pathfinder.GetDirectPath(GridBasedPosition, loc);
        if (newPath == null) return;
        path = newPath;

        pathIterator = path.GetEnumerator();
        PathComplete = false;
    }

    public void SetCardinalDestination(Vector2Int loc)
    {
        Vector2Int? destination = pathfinder.GetClosestReachableCardinalLocation(GridBasedPosition, loc);
        if(destination == null) return;

        SetDestination((Vector2Int)destination);
    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
        GridBasedPosition = Vector2Int.RoundToInt(pos);
    }

    public void StopMoving()
    {
        SetDestination(GridBasedPosition);
    }

    public void LookAt(Vector2 loc)
    {
        LookInDirection(loc - (Vector2)transform.position);
    }

    public void LookInDirection(Vector2 dir)
    {
        if(dir.sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }

        //Rounds the direction vector to the nearest axis vector (up/down/left/right)
        Vector2 axisVector = new();
        if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            axisVector.x = Mathf.Sign(dir.x);
        }
        else
        {
            axisVector.y = Mathf.Sign(dir.y);
        }

        FacingDir = axisVector;
        guardAnimation.LookInDirection(axisVector);
    }

    IEnumerator GuardMovementHandler()
    {
        gridManager.BlockTile(GridBasedPosition);

        while (true)
        {
            while(pathIterator != null && pathIterator.MoveNext())
            {
                if (shouldStopMoving) 
                {
                    yield return null; 
                    continue; 
                }

                if(pathIterator.Current.pos != GridBasedPosition)
                {
                    float waitTime = 0f;
                    //no idea what causes an nre here
                    while(!gridManager.CanMove(pathIterator.Current.pos, false) && waitTime < maxTileBlockWaitSeconds)
                    {
                        waitTime += Time.fixedDeltaTime;
                        yield return new WaitForFixedUpdate();
                    }

                    GridBasedPosition = pathIterator.Current.pos;
                    yield return StartCoroutine(MoveTo(GridBasedPosition));
                }

                if(shouldStopMoving)
                {
                    shouldStopMoving = false;
                    break;
                }
            }

            PathComplete = true;
            yield return null;
        }
    }

    IEnumerator MoveTo(Vector2Int loc)
    {
        Vector2Int prevTile = Vector2Int.RoundToInt(transform.position);
        gridManager.BlockTile(loc);

        guardAnimation.SetMoving(true);
        while(Vector2.Distance((Vector2)transform.position, loc) > maxPositionalError)
        {
            LookAt(loc);

            transform.position = Vector2.MoveTowards(transform.position, loc, Time.fixedDeltaTime * speed);
            yield return new WaitForFixedUpdate();
        }
        guardAnimation.SetMoving(false);

        gridManager.UnblockTile(prevTile);
    }

    public void LoadSettings(GuardSettings settings)
    {
        patrolSpeed = settings.patrolSpeed;
        investigationSpeed = settings.investigationSpeed;
        SetSpeed(false);
    }

    public void SetCrouch(bool crouching)
    {
        IsCrouching = crouching;
        guardAnimation.SetCrouching(IsCrouching);
    }
}
