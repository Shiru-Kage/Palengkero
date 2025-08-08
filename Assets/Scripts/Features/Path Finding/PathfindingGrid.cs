using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class PathfindingGrid : MonoBehaviour
{
    public static PathfindingGrid Instance { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] private Grid unityGrid;
    [SerializeField] private Vector2Int size = new Vector2Int(10, 10);
    public Vector2Int GetGridSize()
    {
        return size;
    }
    [SerializeField] private LayerMask obstacleMask;

    private bool[,] walkable;

    private void Awake()
    {
        if (!Application.isPlaying) return;
        Instance = this;
    }

    public void GenerateGrid()
    {
        walkable = new bool[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 worldPos = GetWorldPosition(x, y);
                walkable[x, y] = !Physics2D.OverlapBox(worldPos, Vector2.one * 0.8f, 0f, obstacleMask);
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return unityGrid.CellToWorld(new Vector3Int(x, y, 0)) + unityGrid.cellSize / 2f;
    }

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3Int cell = unityGrid.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }

    public bool IsWalkable(int x, int y)
    {
        return x >= 0 && y >= 0 && x < size.x && y < size.y && walkable[x, y];
    }

    private void OnDrawGizmos()
    {
        if (unityGrid == null) return;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 worldPos = GetWorldPosition(x, y);
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(worldPos, unityGrid.cellSize * 0.95f);

                bool blocked;
    #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    blocked = Physics2D.OverlapBox(worldPos, Vector2.one * 0.8f, 0f, obstacleMask);
                }
                else
    #endif
                {
                    blocked = !IsWalkable(x, y); // Use runtime grid data
                }

                Gizmos.color = blocked ? new Color(1f, 0f, 0f, 0.4f) : new Color(0f, 1f, 0f, 0.2f);
                Gizmos.DrawCube(worldPos, unityGrid.cellSize * 0.9f);
            }
        }
    }

}
