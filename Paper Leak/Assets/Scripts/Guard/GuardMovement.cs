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
    float patrolSpeed = 1f;
    [SerializeField] float maxPositionalError = 0.01f;
    [SerializeField] LayerMask excludedLayersInPathfinding;
    float speed;

    /// <summary>
    /// Holds the location of the tile that the guard is at/currently travelling to.
    /// </summary>
    public Vector2Int GridBasedPosition { get; private set; }

    List<Node> path = new();
    IEnumerator<Node> pathIterator;
    public bool PathComplete { get; private set; } = true;
    bool shouldStopMoving = false;

    Pathfinder pathfinder;
    GridManager gridManager;

    void Awake()
    {
        pathfinder = FindFirstObjectByType<Pathfinder>();
        gridManager = pathfinder.GetComponent<GridManager>();
    }

    void Start()
    {
        SetSpeed();
        GridBasedPosition = Vector2Int.RoundToInt(transform.position);

        StartCoroutine(GuardMovementHandler());
    }

    void SetSpeed()
    {
        speed = patrolSpeed;
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
        transform.rotation = Quaternion.LookRotation(Vector3.forward, loc - (Vector2)transform.position);
    }

    IEnumerator GuardMovementHandler()
    {
        while (true)
        {
            while(pathIterator != null && pathIterator.MoveNext())
            {
                if (shouldStopMoving) 
                {
                    yield return null; 
                    continue; 
                }

                GridBasedPosition = pathIterator.Current.pos;
                yield return StartCoroutine(MoveTo(GridBasedPosition));

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
        while(Vector2.Distance((Vector2)transform.position, loc) > maxPositionalError)
        {
            LookAt(loc);
            while(!gridManager.CanMove(loc, false))
            {
                yield return new WaitForFixedUpdate();
            }

            transform.position = Vector2.MoveTowards(transform.position, loc, Time.fixedDeltaTime * speed);
            yield return new WaitForFixedUpdate();
        }
    }

    public void LoadSettings(GuardSettings settings)
    {
        patrolSpeed = settings.patrolSpeed;
        SetSpeed();
    }
}
