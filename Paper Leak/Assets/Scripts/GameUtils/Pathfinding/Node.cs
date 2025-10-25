using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public readonly Vector2Int pos; //The world position represented by this node
    public readonly Node previous;  //The preceding node in the A* tree
    public readonly int distanceFromSource;          //The distance so far from the source node, used in cost calculation

    public Node(Vector2Int pos, Node previous)
    {
        this.pos = pos;
        this.previous = previous;
        distanceFromSource = (previous == null) ? 0 : previous.distanceFromSource + 1;
    }

    public Node(Vector2Int pos) : this(pos, null) { }

    //Two nodes are equal if and only if their positions are equal
    public override bool Equals(object other) 
    { 
        if(other == null) return false;
        if(other is not Node) return false;

        return pos == ((Node)other).pos; 
    }

    public List<Node> GetNeighbours()
    {
        if (previous == null)
        {
            return new List<Node>
            {
                new(pos + Vector2Int.up, this),
                new(pos + Vector2Int.right, this),
                new(pos + Vector2Int.left, this),
                new(pos + Vector2Int.down, this)
            };
        }
        else
        {
            //Prioritizes going in the same direction for more natural-looking pathfinding behaviour
            Vector2Int currentDir = pos - previous.pos;
            Vector2Int orthogonal = new(currentDir.y, currentDir.x);

            return new List<Node>
            {
                new(pos + currentDir, this),
                new(pos + orthogonal, this),
                new(pos - orthogonal, this),
                new(pos - currentDir, this)
            };
        }

        //return new List<Node>
        //{
        //    new(pos + Vector2Int.up, this),
        //    new(pos + Vector2Int.right, this),
        //    new(pos + Vector2Int.left, this),
        //    new(pos + Vector2Int.down, this)
        //};
    }

    //Returns the cost of exploring this node using the Manhattan distance heuristic
    public int GetCost(Vector2Int end)
    {
        int x = Mathf.Abs(pos.x - end.x);
        int y = Mathf.Abs(pos.y - end.y);
        return distanceFromSource + x + y;
    }

    public int GetCost(Node node) => GetCost(node.pos);
    
    //Nodes are hashed according to their positions
    public override int GetHashCode() => pos.GetHashCode();
}
