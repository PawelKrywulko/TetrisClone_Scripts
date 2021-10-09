using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform emptyCell;
    [SerializeField] private int height = 30;
    [SerializeField] private int width = 10;
    [SerializeField] private int header = 8;
    [SerializeField] private ParticlePlayer[] rowGlowsFx = new ParticlePlayer[4];

    public int CompletedRows { get; private set; } = 0;
    private Transform[,] _grid;

    private void Awake()
    {
        _grid = new Transform[width, height];
    }

    private void Start()
    {
        DrawEmptyCells();
    }

    private void DrawEmptyCells()
    {
        if (!emptyCell) Debug.Log("Please assign empty cell prefab");
        
        for (int y = 0; y < height - header; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var clone = Instantiate(emptyCell, new Vector3(x, y), Quaternion.identity, transform);
                clone.name = $"Board Space {x}.{y}";
            }
        }
    }

    private bool IsWithinBoard(Vector2Int position)
    {
        return position.x >= 0 && position.x < width && position.y >= 0;
    }

    private bool IsOccupied(Vector2Int position, Shape shape)
    {
        var place = _grid[position.x, position.y];
        return place != null && place.parent != shape.transform;
    }

    private bool IsRowComplete(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (_grid[x,y] == null)
            {
                return false;
            }
        }

        return true;
    }

    private void ClearRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (_grid[x,y] != null)
            {
                Destroy(_grid[x,y].gameObject);
            }

            _grid[x, y] = null;
        }
    }

    private void ShiftOneRowDown(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (_grid[x,y] != null)
            {
                _grid[x, y - 1] = _grid[x, y];
                _grid[x, y] = null;
                _grid[x, y-1].position += Vector3.down;
            }
        }
    }

    private void ShiftRowsDown(int startY)
    {
        for (int i = startY; i < height; i++)
        {
            ShiftOneRowDown(i);
        }
    }

    public IEnumerator ClearAllRows()
    {
        CompletedRows = 0;

        for (int y = 0; y < height; y++)
        {
            if (!IsRowComplete(y)) continue;
            
            ClearRowsFx(CompletedRows, y);
            CompletedRows++;
        }

        yield return new WaitForSeconds(1f);
        for (int y = 0; y < height; y++)
        {
            if (!IsRowComplete(y)) continue;
            
            ClearRow(y);
            ShiftRowsDown(y + 1);
            y--;
        }
    }

    public bool IsValidPosition(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            Vector2Int position = Vector2Int.RoundToInt(child.position);
            if (!IsWithinBoard(position))
            {
                return false;
            }

            if (IsOccupied(position, shape))
            {
                return false;
            }
        }

        return true;
    }

    public void StoreShapeInGrid(Shape shape)
    {
        if (shape == null) return;

        foreach (Transform child in shape.transform)
        {
            Vector2Int position = Vector2Int.RoundToInt(child.position);
            _grid[position.x, position.y] = child;
        }
    }

    public bool IsOverLimit(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            if (child.transform.position.y + 1 >= height - header)
            {
                return true;
            }
        }

        return false;
    }

    private void ClearRowsFx(int index, int y)
    {
        var rowGlowFx = rowGlowsFx[index];
        if (!rowGlowFx) return;

        rowGlowFx.transform.position = new Vector3(0, y, -2);
        rowGlowFx.Play();
    }
}
