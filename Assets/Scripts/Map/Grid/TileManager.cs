using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager
{
    private GridManager _gridManager;
    private Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, TileData> _tileData = new Dictionary<Vector2Int, TileData>();
    private HashSet<Vector2Int> _occupiedTiles = new HashSet<Vector2Int>();

    public Dictionary<Vector2Int, TileData> TileDataInfo => _tileData;

    public TileManager(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public void InitializeTiles(GameObject tilePrefab, Transform parent)
    {
        for (int x = 0; x < _gridManager.Grid.GetWidth(); x++)
        {
            for (int y = 0; y < _gridManager.Grid.GetHeight(); y++)
            {
                Vector3 tilePosition = _gridManager.GetTileWorldPosition(new Vector2Int(x, y));
                GameObject tile = GameObject.Instantiate(tilePrefab, tilePosition, quaternion.identity, parent);
                _tiles[new Vector2Int(x, y)] = tile;
                _tileData[new Vector2Int(x, y)] = new TileData(TileType.Empty, null);
                tile.transform.localScale *= _gridManager.Grid.Setting.CellSize;
            }
        }
    }

    public void PlaceStartAndEndPoints()
    {
        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(_gridManager.Grid.GetWidth() - 1, _gridManager.Grid.GetHeight() - 1);
          
        _occupiedTiles.Add(start);
        _occupiedTiles.Add(end);

        MarkTile(start, Color.cyan, TileType.Start);
        MarkTile(end, Color.magenta, TileType.End);
    }

    public void PlaceDangerAndInterestPoints(int dangerPointsCount, int interestPointsCount)
    {
        PlacePoints(dangerPointsCount, Color.red, TileType.Danger);
        PlacePoints(interestPointsCount, Color.yellow, TileType.Interest);
    }

    public void GenerateWalls(int wallCount, PathfindingManager pathfinding)
    {
        List<Vector2Int> wallPositions = new List<Vector2Int>();
        var startTime = Time.realtimeSinceStartup;
        var allPaths = GetAllPaths(pathfinding);

        while (wallPositions.Count < wallCount)
        {
            Vector2Int randomPoint = GetRandomTile();

            if (IsStartOrEndPoint(randomPoint) || _occupiedTiles.Contains(randomPoint))
                continue;

            _occupiedTiles.Add(randomPoint);
            MarkTile(randomPoint, Color.gray, TileType.Wall);

            _tileData[randomPoint].IsWalkable = false;

            if (!IsPathsValidAfterWall(pathfinding, allPaths))
            {
                RestoreTile(randomPoint);
                _occupiedTiles.Remove(randomPoint);
            }
            else
            {
                wallPositions.Add(randomPoint);
            }
        }
        Debug.Log($"Duration Total = {((Time.realtimeSinceStartup - startTime) * 1000f).ToString("F2")} ms");

        for (int x = 0; x < _gridManager.Grid.GetWidth(); x++)
        {
            for (int y = 0; y < _gridManager.Grid.GetHeight(); y++)
            {
                var tileData = _tileData[new Vector2Int(x, y)];
                var tile = _tiles[new Vector2Int(x, y)];
                if (tileData.Type == TileType.Empty)
                {
                    GameObject.Destroy(tile.gameObject);
                    continue;
                }

                var boxCollider = _tiles[new Vector2Int(x, y)].AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = tileData.Type != TileType.Wall;
                var spriteRenderer = tile.GetComponentInChildren<SpriteRenderer>();
                spriteRenderer.sortingOrder = 1;
            }
        }
    }

    private bool IsPathsValidAfterWall(PathfindingManager pathfinding, List<(Vector2Int start, Vector2Int end)> paths)
    {
        foreach (var (start, end) in paths)
        {
            var path = pathfinding.GeneratePath(new FindPathSetting
            {
                StartX = start.x,
                StartY = start.y,
                EndX = end.x,
                EndY = end.y,
                IsWalkableArray = GetWalkableArray()
            });

            if (path == null || path.Count == 0)
                return false;
        }

        return true;
    }

    private bool IsStartOrEndPoint(Vector2Int position) => 
        _tileData[position].Type == TileType.Start || _tileData[position].Type == TileType.End;

    private void RestoreTile(Vector2Int position)
    {
        _tileData[position].Type = TileType.Empty;
        _tileData[position].IsWalkable = true;

        if (_tiles.ContainsKey(position))
        {
            var renderer = _tiles[position].GetComponentInChildren<SpriteRenderer>();
            if (renderer != null)
                renderer.color = Color.white;
        }
    }

    private List<(Vector2Int start, Vector2Int end)> GetAllPaths(PathfindingManager pathfinding)
    {
        List<(Vector2Int, Vector2Int)> paths = new List<(Vector2Int, Vector2Int)>
        {
            (new Vector2Int(0, 0), new Vector2Int(_gridManager.Grid.GetWidth() - 1, _gridManager.Grid.GetHeight() - 1))
        };

        foreach (var tile in _tileData)
        {
            if (tile.Value.Type == TileType.Interest || tile.Value.Type == TileType.Danger)
            {
                paths.Add((new Vector2Int(0, 0), tile.Key));
            }
        }

        return paths;
    }

    public bool[] GetWalkableArray()
    {
        bool[] walkableArray = new bool[_gridManager.Grid.GetWidth() * _gridManager.Grid.GetHeight()];

        foreach (var tile in _tileData)
        {
            int index = tile.Key.x + tile.Key.y * _gridManager.Grid.GetWidth();
            walkableArray[index] = tile.Value.IsWalkable;
        }

        return walkableArray;
    }

    private void PlacePoints(int count, Color color, TileType type)
    {
        if (color == Color.red)
        {
            color = new Color(1f, 0f, 0f, 0);
        }
        for (int i = 0; i < count; i++)
        {
            Vector2Int randomPoint = GetRandomTile();

            while (_occupiedTiles.Contains(randomPoint))
            {
                randomPoint = GetRandomTile();
            }

            _occupiedTiles.Add(randomPoint);
            MarkTile(randomPoint, color, type);
        }
    }

    private void MarkTile(Vector2Int position, Color color, TileType type)
    {
        if (_tiles.ContainsKey(position))
        {
            var renderer = _tiles[position].GetComponentInChildren<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = color;
            }

            if (_tileData.ContainsKey(position))
            {
                _tileData[position].Type = type;
            }
        }
    }

    private Vector2Int GetRandomTile()
    {
        return new Vector2Int(
            Random.Range(1, _gridManager.Grid.GetWidth()),
            Random.Range(1, _gridManager.Grid.GetHeight()));
    }
    
    public Task GetTaskForTile(Vector2Int position)
    {
        return _tileData.ContainsKey(position) ? _tileData[position].AssignedTask : null;
    }

    public enum TileType
    {
        Empty,
        Start,
        End,
        Danger,
        Interest,
        Wall
    }

    public class TileData
    {
        public TileType Type;
        public bool IsWalkable;
        public Task AssignedTask;

        public TileData(TileType type, Task task)
        {
            Type = type;
            AssignedTask = task;
            IsWalkable = type != TileType.Wall;
        }
    }
}