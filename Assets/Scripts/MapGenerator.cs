using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GridSetting settingGrid;
    [SerializeField] private int dangerPointsCount = 3; // Количество точек опасности
    [SerializeField] private int interestPointsCount = 3; // Количество точек интереса
    [SerializeField] private bool isAllowDiagonal;
    [SerializeField] private int wallCount;

    private Pathfinding _pathfinding;
    private Grid<GridObject> _grid;
    private int[,] _map;
    private Vector2Int _startPoint;
    private Vector2Int _endPoint;

    private void Awake()
    {
        _grid = new Grid<GridObject>(settingGrid, (g, x, y) => new GridObject(g, x, y));
        _pathfinding = new Pathfinding(_grid);
        InitializeMap();
        PlaceStartAndEndPoints();
        PlaceDangerAndInterestPoints();
        GenerateWallsIfCan(wallCount);
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateMap(true);

        if (Input.GetMouseButtonDown(0))
            _pathfinding.ShowPathOnClick();
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
        int totalCells = settingGrid.Width * settingGrid.Height;
        int startToEndPathCells = GeneratePath()?.Count ?? 0;
    
        return totalCells - startToEndPathCells; 
    }

    private void CreateRandomWalls(int wallCount)
    {
        List<Vector2Int> wallPositions = new List<Vector2Int>();

        for (int i = 0; i < wallCount; i++)
        {
            Vector2Int randomPosition = GetRandomPoint();
            if (!IsStartOrEndPoint(randomPosition) && TryPlaceWall(randomPosition, wallPositions))
            {
                wallPositions.Add(randomPosition);
            }
        }

        if (GeneratePath() == null) // Проверяем наличие пути
        {
            RestoreWalkableTiles(wallPositions);
            CreateRandomWalls(wallCount); // Рекурсивный вызов
            return;
        }

        PlaceWallsOnMap(wallPositions);
    }

    private bool IsStartOrEndPoint(Vector2Int position)
    {
        return position == _startPoint || position == _endPoint;
    }

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

    private void RestoreWalkableTiles(List<Vector2Int> wallPositions)
    {
        foreach (var position in wallPositions)
        {
            var gridObject = _grid.GetGridObject(position.x, position.y);
            gridObject.isWalkable = true; // Удаляем стены
        }
    }

    private void PlaceWallsOnMap(List<Vector2Int> wallPositions)
    {
        foreach (var position in wallPositions)
        {
            float cellSize = _grid.GetCellSize();
            Vector3 gridOrigin = _grid.GetOriginPosition();
            Vector3 currentNodeCenter = gridOrigin + new Vector3(position.x, position.y) * cellSize + new Vector3(cellSize / 2, cellSize / 2);
            CreateTile(currentNodeCenter, Color.grey, 2);
            _map[position.x, position.y] = -2; // Помечаем как стену
        }
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
            for (int y = 0; y < settingGrid.Height; y++)
                _map[x, y] = -1; // Пропасти
    }

    private void PlaceStartAndEndPoints()
    {
        _startPoint = new Vector2Int(0, 0);
        _endPoint = new Vector2Int(settingGrid.Width - 1, settingGrid.Height - 1);
        MarkPointOnMap(_startPoint, 0); // Начальная точка
        MarkPointOnMap(_endPoint, 0); // Конечная точка
    }

    private void MarkPointOnMap(Vector2Int point, int value)
    {
        _map[point.x, point.y] = value;
    }

    private void PlaceDangerAndInterestPoints()
    {
        PlacePoints(dangerPointsCount, 101, (x) => x > 100); // Точки опасности
        PlacePoints(interestPointsCount, 1, (x) => x > 1); // Точки интереса
    }

    private void PlacePoints(int count, int minValue, System.Predicate<int> condition)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int point = GetRandomPoint();
            while (!_grid.GetGridObject(point.x, point.y).isWalkable)
                point = GetRandomPoint();
            _map[point.x, point.y] = Random.Range(minValue, minValue + 100);
        }
    }

    private Vector2Int GetRandomPoint()
    {
        int x = Random.Range(1, settingGrid.Width - 1);
        int y = Random.Range(1, settingGrid.Height - 1);
        return new Vector2Int(x, y);
    }

    private List<GridObject> GeneratePath()
    {
        var pathSetting = new FindPathSetting
        {
            StartX = _startPoint.x,
            StartY = _startPoint.y,
            EndX = _endPoint.x,
            EndY = _endPoint.y
        };

        List<GridObject> path = _pathfinding.FindPath(pathSetting, isAllowDiagonal);
        if (path != null)
            foreach (var node in path)
                _map[node.x, node.y] = 1; // Путь

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
            case 0 when x == _startPoint.x && y == _startPoint.y: // Стартовая точка
                CreateTile(currentNodeCenter, Color.cyan, 1);
                break;
            case 0 when x == _endPoint.x && y == _endPoint.y: // Конечная точка
                CreateTile(currentNodeCenter, Color.magenta, 1);
                break;
            case int n when n > 100: // Точки опасности
                CreateTile(currentNodeCenter, Color.red);
                break;
            case int n when n > 1: // Точки интереса
                CreateTile(currentNodeCenter, Color.yellow);
                break;
            case 1: // Ячейка пути
                CreateTile(currentNodeCenter, Color.white);
                break;
            default: // Пустая клетка
                CreateTile(currentNodeCenter, Color.black);
                break;
        }
    }
}
