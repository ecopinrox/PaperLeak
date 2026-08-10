using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class GuardDistractionSensor : MonoBehaviour
{
    [Range(0f, 180f)] float frontalViewAngle = 30f;
    [Range(0f, 180f)] float peripheralViewAngle = 75f;
    float frontalViewRadius = 7f;
    float peripheralViewRadius = 4f;
    [Range(0f, 1f)] float dangerZoneMultiplier = 0.6f;
    float soundAlertDistanceMultiplier;

    [SerializeField] LayerMask distractionColliderMask;

    readonly List<VisualDistraction> visualDistractionList = new();
    readonly HashSet<Distraction> knownDistractionList = new();

    Pathfinder pathfinder;
    PlayerDistraction playerDistraction;
    GridManager gridManager;

    GuardMovement guardMovement;

    CircleCollider2D guardCollider;
    ContactFilter2D contactFilter = new();

    void Awake()
    {
        playerDistraction = FindFirstObjectByType<PlayerDistraction>();
        gridManager = FindFirstObjectByType<GridManager>();
        pathfinder = FindFirstObjectByType<Pathfinder>();

        guardMovement = GetComponent<GuardMovement>();

        guardCollider = GetComponent<CircleCollider2D>();
        contactFilter.layerMask = distractionColliderMask.value;
    }

    public void AddVisualDistraction(VisualDistraction visualDistraction)
    {
        visualDistractionList.Add(visualDistraction);
    }

    public void RegisterDistraction(Distraction distraction)
    {
        if (distraction is PlayerDistraction)
        {
            return;
        }

        if(distraction is VisualDistraction vd)
        {
            visualDistractionList.Remove(vd);
        }
        knownDistractionList.Add(distraction);
    }

    public Distraction GetDistraction(bool isCrouched)
    {
        Distraction player = GetPlayerDistraction(isCrouched);
        if (player != null)
        {
            return player; 
        }

        Distraction vd = GetVisualDistraction(isCrouched);
        Distraction od = GetOverlappingDistraction();

        if (vd == null && od == null)
        {
            return null;
        }

        if (vd == null || od == null)
        {
            return (od == null) ? vd : od;
        }

        return (vd.Priority > od.Priority) ? vd : od;
    }

    Distraction GetVisualDistraction(bool isCrouched)
    {
        Distraction retval = null;
        
        foreach(VisualDistraction distraction in visualDistractionList)
        {
            float multiplier = distraction.ViewDistanceMultiplier;
            Vector2 realDistractionPosition = distraction.transform.position;
            if (!CheckInViewRegion(realDistractionPosition, multiplier)) continue;
            if (CheckRaycastHit(realDistractionPosition, isCrouched)) continue;
            if (!IsDistractionReachable(distraction)) continue;

            if (retval != null && distraction.Priority <= retval.Priority) continue;
            
            retval = distraction;
        }

        return knownDistractionList.Contains(retval) ? null : retval;
    }

    Distraction GetPlayerDistraction(bool isCrouched)
    {

#if UNITY_EDITOR
        if (playerDistraction.IsInvisible) return null;
#endif

        Vector2Int dPos = playerDistraction.Position;
        Vector2 realPos = playerDistraction.transform.position;

        float multiplier = playerDistraction.ViewDistanceMultiplier;
        if (!CheckInViewRegion(dPos, multiplier)) return null;
        if (CheckRaycastHit(realPos, isCrouched)) return null;
        if(!IsDistractionReachable(playerDistraction)) return null;

        if(CheckInViewRegion(playerDistraction.Position, dangerZoneMultiplier)
            && !CheckRaycastHitGlass(playerDistraction.Position))
        {
            GameStateManager.onPlayerDiscovered();
        }

        return playerDistraction;
    }

    Distraction GetOverlappingDistraction()
    {
        List<Collider2D> overlaps = new();
        Physics2D.OverlapCollider(guardCollider, overlaps);

        Distraction retval = null;
        foreach(Collider2D collider in overlaps)
        {
            if(!collider.TryGetComponent(out Distraction distraction))
            {
                if (!collider.TryGetComponent(out SoundEmitter se))
                {
                    continue;
                }
                
                distraction = se.GetOriginDistraction();
                float maxWalkingDistance = se.GetDistractionRadius() * soundAlertDistanceMultiplier;

                Vector2Int? dest = pathfinder.GetClosestReachableCardinalLocation(guardMovement.GridBasedPosition, distraction.Position);
                if (dest == null)
                {
                    continue;
                }
                
                if(!pathfinder.IsReachable(guardMovement.GridBasedPosition, (Vector2Int)dest))
                {
                    continue;
                }

                int distance = pathfinder.GetRealDistance(guardMovement.GridBasedPosition, (Vector2Int)dest); 
                if (distance > maxWalkingDistance)
                {
                    continue;
                }
            }

            if(retval == null || distraction.Priority > retval.Priority) 
                retval = distraction;
        }

        return knownDistractionList.Contains(retval) ? null : retval;
    }

    float GetDistanceFromLocation(Vector2 loc)
    {
        return Vector2.Distance(loc, (Vector2)transform.position);
    }

    bool CheckRaycastHit(Vector2 loc, bool isCrouched)
    {
        int standingRaycastHitMask = LayerMask.GetMask("Wall", "Crawlable");
        int crouchingRaycastHitMask = LayerMask.GetMask("Wall");

        if (!isCrouched && gridManager.IsLocationInMask(Vector2Int.RoundToInt(loc), LayerMask.GetMask("Crawlable"))) return true;

        Vector2 direction = loc - (Vector2)transform.position;
        float distance = GetDistanceFromLocation(loc);

        RaycastHit2D hit;
        if (!isCrouched) hit = Physics2D.Raycast(transform.position, direction, distance, crouchingRaycastHitMask); //standingRaycastHitMask
        else hit = Physics2D.Raycast(transform.position, direction, distance, crouchingRaycastHitMask);

        if (hit.collider != null) return true;
        return false;
    }

    bool CheckRaycastHitGlass(Vector2 loc)
    {
        int glassRaycastHitMask = LayerMask.GetMask("Transparent");

        Vector2 direction = loc - (Vector2)transform.position;
        float distance = GetDistanceFromLocation(loc);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, glassRaycastHitMask);

        if (hit.collider != null) return true;
        return false;
    }

    bool CheckInViewRegion(Vector2 loc, float multiplier)
    {
        float theta = Vector3.Angle(guardMovement.FacingDir, loc - (Vector2)transform.position);
        float distance = GetDistanceFromLocation(loc);

        if(theta <= frontalViewAngle && distance <= frontalViewRadius * multiplier) return true;
        if(theta <= peripheralViewAngle && distance <= peripheralViewRadius * multiplier) return true;
        return false;
    }

    bool IsDistractionReachable(Distraction distraction)
    {
        return pathfinder.IsReachableCardinal(guardMovement.GridBasedPosition, distraction.Position);
    }

    public void LoadSettings(GuardSettings guardSettings)
    {
        frontalViewAngle = guardSettings.frontalViewAngle;
        peripheralViewAngle = guardSettings.peripheralViewAngle;
        frontalViewRadius = guardSettings.frontalViewRadius;
        peripheralViewRadius = guardSettings.peripheralViewRadius;
        dangerZoneMultiplier = guardSettings.dangerZoneMultiplier;
        soundAlertDistanceMultiplier = guardSettings.soundAlertDistanceMultiplier;
    }
}
