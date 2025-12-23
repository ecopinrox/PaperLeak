using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Pathfinder : MonoBehaviour 
{
    [Header("Masks")]
    [SerializeField] LayerMask transparentMask;
    [SerializeField] LayerMask opaqueMask;
    [SerializeField] LayerMask unwalkableMask;

    [Header("Region mapping")]
    [SerializeField] Vector2Int startPos = new(1, 1);

    [Header("Debug")]
    [SerializeField] bool showRegionMappings;

    GridManager gridManager;
    TileRegionMarker tileRegionMarker;

    const int outOfBoundsRegion = -1;
    const int opaqueRegion = 0;
    const int transparentRegion = 1;
    const int lowestWalkableRegion = 2;

    readonly IndexedSet<Vector2Int> regions = new();

    private void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        tileRegionMarker = FindFirstObjectByType<TileRegionMarker>();
    }

    private void Start()
    {
        //StartCoroutine(DebugMarkTiles(new(6, 45)));
        MarkTiles(startPos);
    }

    void MarkTiles(Vector2Int startPos)
    {
        ClearRegionMap();
        IndexedSet<Node> frontier = new();

        Node startNode = new(startPos);
        AssignRegion(startPos, lowestWalkableRegion);
        frontier.Add(startNode, lowestWalkableRegion);

        while (frontier.Count > 0)
        {
            Node node = frontier.PopItem(frontier.GetHighestIndex());
            int currentRegion = GetAssignedRegion(node.pos);
            List<Node> neighbours = node.GetNeighbours();

            foreach (Node neighbour in neighbours)
            {
                if (TryGetAssignedRegion(neighbour.pos, out _))
                {
                    continue;
                }

                int region = (currentRegion >= lowestWalkableRegion) ? 
                    GetRegionNumber(neighbour.pos, currentRegion) : 
                    GetRegionNumber(neighbour.pos, frontier.HighestIndexUsed + 1);

                if(region == outOfBoundsRegion)
                {
                    continue;
                }

                AssignRegion(neighbour.pos, region);
                frontier.Add(neighbour, region);
            }
        }
    }

    public bool IsReachable(Vector2Int start, Vector2Int end)
    {
        try
        {
            return regions.GetIndex(start) == regions.GetIndex(end);
        }
        catch(Exception e)
        {
            Debug.Log($"Reachability check failed with parameters {start}, {end}");
            throw e;
        }
    }

    public bool IsReachableCardinal(Vector2Int start, Vector2Int end)
    {
        if(GetClosestReachableCardinalLocation(start, end) is not Vector2Int dest)
        {
            return false;
        }

        if(!IsReachable(start, dest))
        {
            return false;
        }

        return true;
    }

    public void AddDoorTilesToRegionList(List<Vector2Int> doorTiles)
    {
        //check whether the door is horizontal or vertical
        bool isHorizontal;
        Vector2Int difference = doorTiles[0] - doorTiles[1];
        isHorizontal = (difference == Vector2Int.left || difference == Vector2Int.right);

        //get the perpendicular vector
        Vector2Int perpendicular = isHorizontal ? Vector2Int.up : Vector2Int.left;

        //get the adjacent regions
        Vector2Int tileA = doorTiles[0] + perpendicular;
        Vector2Int tileB = doorTiles[0] - perpendicular;

        //merge the regions if there are two adjacent regions, else just get the region that is adjacent to the door
        int finalRegion;
        if (regions.Contains(tileA) && regions.Contains(tileB))
        {
            int regionA = GetAssignedRegion(tileA);
            int regionB = GetAssignedRegion(tileB);
            MergeRegions(regionA, regionB);
            finalRegion = regionA;
        }
        else
        {
            finalRegion = regions.Contains(tileA) ? GetAssignedRegion(tileA) : GetAssignedRegion(tileB);
        }

        //add the door tiles to region A
        foreach (Vector2Int doorTile in doorTiles)
        {
            AssignRegion(doorTile, finalRegion);
        }
    }

    public List<Node> GetDirectPath(Vector2Int source, Vector2Int destination)
    {
        if (!IsReachable(source, destination)) return null;

        Node startNode = new(source);
        Node endNode = new(destination);

        HashSet<Node> visited = new();
        HashSet<Node> frontier = new() { startNode };

        while (frontier.Count > 0)
        {
            Node current = GetHighestPriorityNode(frontier, endNode);
            visited.Add(current);

            if(current.Equals(endNode)) return GeneratePath(current);

            List<Node> neighbours = current.GetNeighbours();

            foreach (Node node in neighbours)
            {
                if(visited.Contains(node) || frontier.Contains(node)) continue;
                if(gridManager.IsLocationInMask(node.pos, unwalkableMask)) continue;

                frontier.Add(node);
            }
        }

        return null;
    }

    public int GetRealDistance(Vector2Int source, Vector2Int destination)
    {
        if (!IsReachable(source, destination)) return -1;

        Node startNode = new(source);
        Node endNode = new(destination);

        HashSet<Node> visited = new();
        HashSet<Node> frontier = new() { startNode };

        while (frontier.Count > 0)
        {
            Node current = GetHighestPriorityNode(frontier, endNode);
            visited.Add(current);

            if(current.Equals(endNode)) return current.distanceFromSource;

            List<Node> neighbours = current.GetNeighbours();

            foreach (Node node in neighbours)
            {
                if(visited.Contains(node) || frontier.Contains(node)) continue;
                if(gridManager.IsLocationInMask(node.pos, unwalkableMask)) continue;

                frontier.Add(node);
            }
        }

        return -1;
    }

    #region DebugCoroutines
    IEnumerator DebugPath(Vector2Int source, Vector2Int destination, int maxLength)
    {
        //Checks if a path exists from the source to the destination
        if (!IsReachable(source, destination)) yield break;

        Node startNode = new(source);
        Node endNode = new(destination);

        HashSet<Node> visited = new();                  //The set of nodes that have already been explored
        HashSet<Node> frontier = new() { startNode };   //The set of nodes that have been visited but are yet to be explored

        while (frontier.Count > 0)
        {
            //Get the lowest-cost node in the frontier and start exploring it
            Node current = GetHighestPriorityNode(frontier, endNode);
            visited.Add(current);

            //StopTimer running if the destination is reached
            if (current.Equals(endNode)) yield break;

            //Gets the 4 neighbours of the current node and adds them to the frontier if valid
            List<Node> neighbours = current.GetNeighbours();
            foreach (Node node in neighbours)
            {
                if (visited.Contains(node) || frontier.Contains(node)) continue;

                //Disregard the neighbour if it is outside the walkable space
                if (gridManager.IsLocationInMask(node.pos, unwalkableMask)) continue;

                if(node.distanceFromSource >= maxLength) continue;

                frontier.Add(node);

                //Draws a line from the current node to its neighbour
                Debug.DrawLine((Vector2)node.pos, (Vector2)node.previous.pos, Color.yellow, 300f, false);

                //100ms delay between consecutive line draws
                yield return new WaitForSeconds(0.01f);
            }
        }

        yield break;
    }

    IEnumerator DebugMarkTiles(Vector2Int startPos)
    {
        float startTime = Time.time;

        ClearRegionMap();
        IndexedSet<Node> frontier = new();

        Node startNode = new(startPos);
        AssignRegion(startPos, lowestWalkableRegion);
        frontier.Add(startNode, lowestWalkableRegion);

        while (frontier.Count > 0)
        {
            Node node = frontier.PopItem(frontier.GetHighestIndex());
            int currentRegion = GetAssignedRegion(node.pos);
            List<Node> neighbours = node.GetNeighbours();

            foreach (Node neighbour in neighbours)
            {
                if (TryGetAssignedRegion(neighbour.pos, out _))
                {
                    continue;
                }

                int region = (currentRegion >= lowestWalkableRegion) ? 
                    GetRegionNumber(neighbour.pos, currentRegion) : 
                    GetRegionNumber(neighbour.pos, frontier.HighestIndexUsed + 1);

                if(region == outOfBoundsRegion)
                {
                    continue;
                }

                AssignRegion(neighbour.pos, region);
                frontier.Add(neighbour, region);
            }
            //yield return new WaitForSeconds(0.001f);
        }
        yield return null;

        Debug.Log($"Mapped all nodes in {Time.time - startTime} seconds");
    }
    #endregion

    public Vector2Int? GetClosestReachableCardinalLocation(Vector2Int source, Vector2Int destination)
    {
        Vector2Int? up = GetClosestReachableLocationInDirection(destination, Vector2Int.up, source);
        Vector2Int? right = GetClosestReachableLocationInDirection(destination, Vector2Int.right, source);
        Vector2Int? left = GetClosestReachableLocationInDirection(destination, Vector2Int.left, source);
        Vector2Int? down = GetClosestReachableLocationInDirection(destination, Vector2Int.down, source);

        List<Node> cardinalLocations = new()
        {
            (up == null) ? null : new((Vector2Int)up),
            (right == null) ? null : new((Vector2Int)right),
            (left == null) ? null : new((Vector2Int)left),
            (down == null) ? null : new((Vector2Int)down)
        };

        cardinalLocations.Sort((x, y) => {
            if (x == null || y == null)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
            }

            int d1 = Mathf.Abs(x.pos.x - destination.x) + Mathf.Abs(x.pos.y - destination.y);
            int d2 = Mathf.Abs(y.pos.x - destination.x) + Mathf.Abs(y.pos.y - destination.y);

            if (d1 - d2 != 0) return d1 - d2;

            //d1 = Mathf.Abs(x.pos.x - source.x) + Mathf.Abs(x.pos.y - source.y);
            //d2 = Mathf.Abs(y.pos.x - source.x) + Mathf.Abs(y.pos.y - source.x);
            d1 = GetRealDistance(x.pos, source);
            d2 = GetRealDistance(y.pos, source);

            return d1 - d2;
        });

        foreach (Node node in cardinalLocations)
        {
            if (node == null) continue;
            if (IsReachable(source, node.pos))
            {
                return node.pos;
            }
        }
        return null;
    }

    Vector2Int? GetClosestReachableLocationInDirection(Vector2Int destination, Vector2Int direction, Vector2Int source)
    {
        for (Vector2Int closest = destination + direction; !gridManager.IsLocationInMask(closest, opaqueMask); closest += direction)
        {
            if (IsReachable(source, closest))
            {
                return closest;
            }
        }

        return null;
    }

    Node GetHighestPriorityNode(HashSet<Node> frontier, Node end)
    {
        int leastCost = int.MaxValue;
        Node leastPriorityNode = null;

        foreach (Node node in frontier)
        {
            if (leastCost > node.GetCost(end))
            {
                leastCost = node.GetCost(end);
                leastPriorityNode = node;
            }
        }

        frontier.Remove(leastPriorityNode);
        return leastPriorityNode;
    }

    List<Node> GeneratePath(Node end)
    {
        List<Node> path = new() { end };

        while (end.previous != null)
        {
            path.Add(end.previous);
            end = end.previous;
        }
        path.Reverse();

        return path;
    }

    void AssignRegion(Vector2Int pos, int region)
    {
        regions.Add(pos, region);
        if(showRegionMappings)
        {
            tileRegionMarker.MarkTile(pos, region);
        }
    }

    int GetRegionNumber(Vector2Int pos, int currentRegion)
    {
        if (gridManager.IsLocationInMask(pos, opaqueMask)) return opaqueRegion;
        if (gridManager.IsLocationInMask(pos, transparentMask)) return transparentRegion;
        if (gridManager.IsWalkable(pos)) return currentRegion;
        return outOfBoundsRegion;
    }

    int GetAssignedRegion(Vector2Int pos)
    {
        return regions.GetIndex(pos);
    }

    bool TryGetAssignedRegion(Vector2Int pos, out int region)
    {
        return regions.TryGetIndex(pos, out region);
    }

    void ClearRegionMap()
    {
        regions.Clear();
    }

    void MergeRegions(int regionA, int regionB)
    {
        if (regionA == regionB) return;

        if(showRegionMappings)
        {
            foreach(Vector2Int loc in regions[regionB])
            {
                tileRegionMarker.MarkTile(loc, regionA);
            }
        }

        regions.Merge(regionA, regionB);
    }
}