using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private class Node
    {
        public Vector2Int pos;
        public int gCost, hCost;
        public int fCost => gCost + hCost;
        public Node parent;

        public Node(Vector2Int pos) => this.pos = pos;
    }

    public static List<Vector3> FindPath(Vector3 startWorld, Vector3 endWorld)
    {
        var grid = PathfindingGrid.Instance;
        Vector2Int start = grid.GetGridPosition(startWorld);
        Vector2Int end = grid.GetGridPosition(endWorld);

        var open = new List<Node>();
        var closed = new HashSet<Vector2Int>();

        Node startNode = new Node(start) { gCost = 0, hCost = Heuristic(start, end) };
        open.Add(startNode);

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.fCost.CompareTo(b.fCost));
            Node current = open[0];

            if (current.pos == end)
                return Retrace(current);

            open.RemoveAt(0);
            closed.Add(current.pos);

            foreach (Vector2Int offset in GetNeighbors())
            {
                Vector2Int neighborPos = current.pos + offset;
                if (!grid.IsWalkable(neighborPos.x, neighborPos.y) || closed.Contains(neighborPos))
                    continue;

                int tentativeG = current.gCost + 10;

                Node neighbor = open.Find(n => n.pos == neighborPos);
                if (neighbor == null)
                {
                    neighbor = new Node(neighborPos)
                    {
                        gCost = tentativeG,
                        hCost = Heuristic(neighborPos, end),
                        parent = current
                    };
                    open.Add(neighbor);
                }
                else if (tentativeG < neighbor.gCost)
                {
                    neighbor.gCost = tentativeG;
                    neighbor.parent = current;
                }
            }
        }

        return null; // no path
    }

    private static int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector2Int> GetNeighbors()
    {
        return new List<Vector2Int>
        {
            new Vector2Int( 1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int( 0, 1),
            new Vector2Int( 0,-1),
        };
    }

    private static List<Vector3> Retrace(Node end)
    {
        List<Vector3> path = new List<Vector3>();
        var grid = PathfindingGrid.Instance;
        Node current = end;

        while (current != null)
        {
            path.Add(grid.GetWorldPosition(current.pos.x, current.pos.y));
            current = current.parent;
        }

        path.Reverse();
        return path;
    }
}
