using System;
using System.Collections.Generic;
using System.Linq;
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

    public Grid<GridObject> GetGrid()
    {
        return _grid;
    }

    public bool IsQuestTile(GridObject obj)
    {
        var mapItem = _map[obj.x, obj.y];
        return mapItem > 2 && mapItem < 100;
    }
    
    public int[,] GetMap()
    {
        return _map;
    }
    
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
        var allPaths = GetAllPathToInterestPont(); // Получаем все пути

        while (wallPositions.Count < wallCount)
        {
            Vector2Int randomPosition = GetRandomPoint();

            while (IsStartOrEndPoint(randomPosition) || !TryPlaceWall(randomPosition, wallPositions))
            {
                randomPosition = GetRandomPoint();
            }

            wallPositions.Add(randomPosition);
            _isWalkableArray[CalculateIndex(randomPosition.x, randomPosition.y)] = false;

            var startTimeCheckPath = Time.realtimeSinceStartup;

            if (DoesWallIntersectPaths(randomPosition, allPaths, out List<List<int2>> intersectingPaths))
            {
                for (var index = 0; index < intersectingPaths.Count; index++)
                {
                    var path = intersectingPaths[index];
                    List<int2> newPath = RecalculatePath(path); // Перестраиваем путь
                    if (newPath.Count > 0)
                    {
                        var indexInAllPath= allPaths.FindIndex(obj => obj == intersectingPaths[index]);
                        allPaths[indexInAllPath] = newPath;
                    }
                    else
                    {
                        // Если хотя бы один путь не существует, удаляем стену
                        RestoreWalkableTiles(wallPositions[^1]);
                        wallPositions.RemoveAt(wallPositions.Count - 1);
                        break; // Прерываем текущий цикл while, чтобы попробовать новую позицию
                    }
                }
            }

            // Debug.Log($"Duration Time Check Path = {((Time.realtimeSinceStartup - startTimeCheckPath) * 1000f).ToString("F2")} ms");
        }

        foreach (var pathNode in allPaths.SelectMany(path => path))
        {
            if(_map[pathNode.x, pathNode.y] == -1)
                _map[pathNode.x, pathNode.y] = 1;// Выставляем пути на карте
        }
        
        Debug.Log($"Duration Total = {((Time.realtimeSinceStartup - startTime) * 1000f).ToString("F2")} ms");
        PlaceWallsOnMap(wallPositions);
        DrawMap();
    }
    
    private bool DoesWallIntersectPaths(Vector2Int wallPosition, List<List<int2>> allPaths,
        out List<List<int2>> intersectingPaths)
    {
        intersectingPaths = new List<List<int2>>();

        // Проверяем все пути на пересечение с текущей стеной
        foreach (var path in allPaths)
        {
            if (path.Contains(new int2(wallPosition.x, wallPosition.y)))
            {
                intersectingPaths.Add(path);
            }
        }

        return intersectingPaths.Count > 0; // Вернуть true, если есть пересечения
    }

    private List<int2> RecalculatePath(List<int2> oldPath)
    {
        Vector2Int endPoint = new Vector2Int(oldPath[0].x, oldPath[0].y);

        return GeneratePath(new FindPathSetting
        {
            StartX = _startPoint.x,
            StartY = _startPoint.y,
            EndX = endPoint.x,
            EndY = endPoint.y,
            IsWalkableArray = _isWalkableArray
        });
    }

    private int CalculateIndex(int x, int y) => x + y * settingGrid.Width;

    private List<List<int2>> GetAllPathToInterestPont()
    {
        List<List<int2>> allPath = new List<List<int2>>
        {
            GeneratePath(new FindPathSetting
            {
                StartX = _startPoint.x,
                StartY = _startPoint.y,
                EndX = _endPoint.x,
                EndY = _endPoint.y,
                IsWalkableArray = _isWalkableArray
            })
        };

        for (int x = 0; x < settingGrid.Width; x++)
        {
            for (int y = 0; y < settingGrid.Height; y++)
            {
                if (_map[x, y] > 1)
                {
                    allPath.Add(GeneratePath(new FindPathSetting
                    {
                        StartX = _startPoint.x,
                        StartY = _startPoint.y,
                        EndX = x,
                        EndY = y,
                        IsWalkableArray = _isWalkableArray
                    }));
                }
            }
        }

        return allPath;
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
        var boxCollider = tile.AddComponent<BoxCollider2D>();
        boxCollider.size *= settingGrid.CellSize;
        boxCollider.isTrigger = color == Color.yellow;
        boxCollider.compositeOperation = boxCollider.isTrigger
            ? Collider2D.CompositeOperation.None
            : Collider2D.CompositeOperation.Merge;
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
            } while (_map[point.x, point.y] != -1);
            _map[point.x, point.y] = Random.Range(minValue, minValue + 100);
        }
    }

    private Vector2Int GetRandomPoint() => 
        new Vector2Int(Random.Range(0, settingGrid.Width), Random.Range(0, settingGrid.Height));

    private List<int2> GeneratePath(FindPathSetting pathSetting)
    {
        List<int2> path = _pathfinding.FindPath(pathSetting, isAllowDiagonal);

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
                // CreateTile(currentNodeCenter, Color.cyan, 1);
                break;
            case 0 when x == _endPoint.x && y == _endPoint.y:
                // CreateTile(currentNodeCenter, Color.magenta, 1);
                break;
            case int n when n > 102:
                CreateTile(currentNodeCenter, Color.red, 1);
                break;
            case int n when n > 1:
                CreateTile(currentNodeCenter, Color.yellow, 1);
                break;
            case 1:
                // CreateTile(currentNodeCenter, Color.white);
                break;
            default:
                // CreateTile(currentNodeCenter, Color.black);
                break;
        }
    }
}
