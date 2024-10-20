using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GridSetting settingGrid;
    [SerializeField] private int dangerPointsCount = 3;
    [SerializeField] private int interestPointsCount = 3;
    [SerializeField] private bool isAllowDiagonal;
    [SerializeField] private int wallCount;

    private PathfindingJob _pathfinding;
    private Grid<GridObject> _grid;
    private int[,] _map;
    private Vector2Int _startPoint;
    private Vector2Int _endPoint;
    private bool[] _isWalkableArray;

    private void Awake()
    {
        InitializeGrid();
        InitializeMap();
        PlaceStartAndEndPoints();
        PlaceDangerAndInterestPoints();
        GenerateWallsIfCan(wallCount);
    }

    private void InitializeGrid()
    {
        _grid = new Grid<GridObject>(settingGrid, (g, x, y) => new GridObject(g, x, y));
        _pathfinding = new PathfindingJob(_grid);
        _isWalkableArray = new bool[settingGrid.Width * settingGrid.Height];
        Array.Fill(_isWalkableArray, true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateMap(true);
    }

    private void GenerateMap(bool isDrawingPath)
    {
        if (isDrawingPath)
            DrawMap();
    }

    private void GenerateWallsIfCan(int wallCount)
    {
        if (wallCount > MaxWallsAvailable())
        {
            Debug.LogError("Too many walls. Cannot block path.");
            return;
        }
        CreateRandomWalls(wallCount);
    }

    private int MaxWallsAvailable()
    {
        int minPathCells = (settingGrid.Width - 1) + (settingGrid.Height - 1);
        return (settingGrid.Width * settingGrid.Height) - minPathCells;
    }

    private void CreateRandomWalls(int wallCount)
    {
        List<Vector2Int> wallPositions = new List<Vector2Int>();
        var startTime = Time.realtimeSinceStartup;
        while (wallPositions.Count < wallCount)
        {
            Vector2Int randomPosition = GetRandomPoint();
            while (IsStartOrEndPoint(randomPosition) || !TryPlaceWall(randomPosition, wallPositions))
            {
                randomPosition = GetRandomPoint();
            }

            wallPositions.Add(randomPosition);
            _isWalkableArray[CalculateIndex(randomPosition.x, randomPosition.y)] = false;

            ClearPathInMap();
            var startTimeCheckPath = Time.realtimeSinceStartup;
            if (!PathExistsToEndPoint() || !PathExistsToInterestAndDangerPoints())
            {
                RestoreWalkableTiles(wallPositions[^1]);
                wallPositions.RemoveAt(wallPositions.Count - 1);
            }
            Debug.Log("Duration Time Check Path = " + ((Time.realtimeSinceStartup - startTimeCheckPath ) * 1000f));
        }
        Debug.Log("Duration Total = " + ((Time.realtimeSinceStartup - startTime ) * 1000f));
        PlaceWallsOnMap(wallPositions);
        PlaceWallsOnMap(wallPositions);
    }

    private int CalculateIndex(int x, int y) => x + y * settingGrid.Width;

    private void ClearPathInMap()
    {
        for (int x = 0; x < settingGrid.Width; x++)
            for (int y = 0; y < settingGrid.Height; y++)
                if (_map[x, y] == 1)
                    _map[x, y] = -1;
    }

    private bool PathExistsToEndPoint() => 
        GeneratePath(new FindPathSetting
        {
            StartX = _startPoint.x,
            StartY = _startPoint.y,
            EndX = _endPoint.x,
            EndY = _endPoint.y,
            IsWalkableArray = _isWalkableArray
        }).Count > 0;

    private bool PathExistsToInterestAndDangerPoints()
    {
        for (int x = 0; x < settingGrid.Width; x++)
        {
            for (int y = 0; y < settingGrid.Height; y++)
            {
                if (_map[x, y] > 1 &&
                    GeneratePath(new FindPathSetting
                    {
                        StartX = _startPoint.x,
                        StartY = _startPoint.y,
                        EndX = x,
                        EndY = y,
                        IsWalkableArray = _isWalkableArray
                    }).Count == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsStartOrEndPoint(Vector2Int position) => 
        position == _startPoint || position == _endPoint;

    private bool TryPlaceWall(Vector2Int position, List<Vector2Int> wallPositions)
    {
        var gridObject = _grid.GetGridObject(position.x, position.y);
        if (gridObject.isWalkable)
        {
            gridObject.isWalkable = false;
            return true;
        }
        return false;
    }

    private void RestoreWalkableTiles(Vector2Int position)
    {
        var gridObject = _grid.GetGridObject(position.x, position.y);
        gridObject.isWalkable = true;
        _isWalkableArray[CalculateIndex(position.x, position.y)] = true;
    }

    private void PlaceWallsOnMap(List<Vector2Int> wallPositions)
    {
        foreach (var position in wallPositions)
        {
            Vector3 currentNodeCenter = GetTilePosition(position);
            CreateTile(currentNodeCenter, Color.grey, 2);
            _map[position.x, position.y] = -2; 
        }
    }

    private Vector3 GetTilePosition(Vector2Int position)
    {
        float cellSize = _grid.GetCellSize();
        Vector3 gridOrigin = _grid.GetOriginPosition();
        return gridOrigin + new Vector3(position.x, position.y) * cellSize + new Vector3(cellSize / 2, cellSize / 2);
    }

    private void CreateTile(Vector3 pos, Color color, int sortingOrder = 0)
    {
        var tile = Instantiate(tilePrefab, transform);
        tile.transform.position = pos;
        var spriteRenderer = tile.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    private void InitializeMap()
    {
        _map = new int[settingGrid.Width, settingGrid.Height];
        for (int x = 0; x < settingGrid.Width; x++)
        {
            for (int y = 0; y < settingGrid.Height; y++)
                _map[x, y] = -1;
        }
    }

    private void PlaceStartAndEndPoints()
    {
        _startPoint = new Vector2Int(0, 0);
        _endPoint = new Vector2Int(settingGrid.Width - 1, settingGrid.Height - 1);
        MarkPointOnMap(_startPoint, 0);
        MarkPointOnMap(_endPoint, 0);
    }

    private void MarkPointOnMap(Vector2Int point, int value) =>
        _map[point.x, point.y] = value;

    private void PlaceDangerAndInterestPoints()
    {
        PlacePoints(dangerPointsCount, 103);
        PlacePoints(interestPointsCount, 2);
    }

    private void PlacePoints(int count, int minValue)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int point;
            do
            {
                point = GetRandomPoint();
            } while (!_grid.GetGridObject(point.x, point.y).isWalkable);
            _map[point.x, point.y] = Random.Range(minValue, minValue + 100);
        }
    }

    private Vector2Int GetRandomPoint() => 
        new Vector2Int(Random.Range(1, settingGrid.Width - 1), Random.Range(1, settingGrid.Height - 1));

    private List<int2> GeneratePath(FindPathSetting pathSetting)
    {
        List<int2> path = _pathfinding.FindPath(pathSetting, isAllowDiagonal);
        if (path.Count > 0)
            foreach (var node in path)
                if (_map[node.x, node.y] == -1)
                    _map[node.x, node.y] = 1;

        return path;
    }

    private void DrawMap()
    {
        float cellSize = _grid.GetCellSize();
        Vector3 gridOrigin = _grid.GetOriginPosition();

        for (int y = 0; y < settingGrid.Height; y++)
        {
            for (int x = 0; x < settingGrid.Width; x++)
            {
                Vector3 currentNodeCenter = gridOrigin + new Vector3(x, y) * cellSize + new Vector3(cellSize / 2, cellSize / 2);
                DrawTileOnMap(x, y, currentNodeCenter);
            }
        }
    }

    private void DrawTileOnMap(int x, int y, Vector3 currentNodeCenter)
    {
        switch (_map[x, y])
        {
            case 0 when x == _startPoint.x && y == _startPoint.y:
                CreateTile(currentNodeCenter, Color.cyan, 1);
                break;
            case 0 when x == _endPoint.x && y == _endPoint.y:
                CreateTile(currentNodeCenter, Color.magenta, 1);
                break;
            case int n when n > 102:
                CreateTile(currentNodeCenter, Color.red, 1);
                break;
            case int n when n > 1:
                CreateTile(currentNodeCenter, Color.yellow, 1);
                break;
            case 1:
                CreateTile(currentNodeCenter, Color.white);
                break;
            default:
                CreateTile(currentNodeCenter, Color.black);
                break;
        }
    }
}
