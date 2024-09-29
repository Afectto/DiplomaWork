using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    private Pathfinding _pathfinding;
    private Grid<GridObject> _grid;
    private int _width;
    private int _height;
    
    private int[,] _map;
    private Vector2Int _startPoint;
    private Vector2Int _endPoint;

    [SerializeField] private int dangerPointsCount = 3; // Количество точек опасности
    [SerializeField] private int interestPointsCount = 3; // Количество точек интереса

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _grid ??= FindAnyObjectByType<CreateGridObject>().Grid;
            _pathfinding = FindAnyObjectByType<Pathfinding>();
            _width = _grid.GetWight();
            _height = _grid.GetHeight();
            GenerateMap(false, true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            _grid ??= FindAnyObjectByType<CreateGridObject>().Grid;
            var mosePosition = Utils.GetMouseWorldPosition();
            var gridObject = _grid.GetGridObject(mosePosition);
            gridObject.isWalkable = !gridObject.isWalkable;
            float cellSize = _grid.GetCellSize();
            Vector3 gridOrigin = _grid.GetOriginPosition();
            Vector3 currentNodeCenter = gridOrigin + new Vector3(gridObject.x, gridObject.y) * cellSize +
                                        new Vector3(cellSize / 2, cellSize / 2);
            CreateTile(currentNodeCenter, Color.grey, 2);
        }
    }
    
    public void GenerateMap(bool allowDiagonal, bool isDrawingPath)
    {
        InitializeMap();
        PlaceStartAndEndPoints();
        PlaceDangerAndInterestPoints();
        
        GeneratePath(allowDiagonal);
        
        float cellSize = _grid.GetCellSize();
        Vector3 gridOrigin = _grid.GetOriginPosition();
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Vector3 currentNodeCenter = gridOrigin + new Vector3(x, y) * cellSize +
                                            new Vector3(cellSize / 2, cellSize / 2);
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

        if (isDrawingPath)
        {
            DrawMap();
        }
    }

    private void CreateTile(Vector3 pos, Color color, int sortingOrder = 0)
    {
        var tile = Instantiate(tilePrefab, transform);
        tile.transform.position = pos;
        tile.GetComponentInChildren<SpriteRenderer>().color = color;
        tile.GetComponentInChildren<SpriteRenderer>().sortingOrder = sortingOrder;
    }
    
    private void InitializeMap()
    {
        _map = new int[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _map[x, y] = -1; // Пропасти
            }
        }
    }

    private void PlaceStartAndEndPoints()
    {
        _startPoint = new Vector2Int(0, 0);
        _endPoint = new Vector2Int(_width - 1, _height - 1);
        _map[_startPoint.x, _startPoint.y] = 0; // Начальная точка
        _map[_endPoint.x, _endPoint.y] = 0; // Конечная точка
    }

    private void PlaceDangerAndInterestPoints()
    {
        for (int i = 0; i < dangerPointsCount; i++)
        {
            Vector2Int point = GetRandomPoint();
            _map[point.x, point.y] = Random.Range(1, 100) + 100; // Точки опасности
        }

        for (int i = 0; i < interestPointsCount; i++)
        {
            Vector2Int point = GetRandomPoint();
            _map[point.x, point.y] = Random.Range(1, 100); // Точки интереса
        }
    }

    private Vector2Int GetRandomPoint()
    {
        int x = Random.Range(1, _width - 1);
        int y = Random.Range(1, _height - 1);
        return new Vector2Int(x, y);
    }

    private List<GridObject> GeneratePath(bool allowDiagonal)
    {
        var pathSetting = new FindPathSetting 
        { 
            StartX = _startPoint.x, 
            StartY = _startPoint.y, 
            EndX = _endPoint.x, 
            EndY = _endPoint.y 
        };

        List<GridObject> path = _pathfinding.FindPath(pathSetting, allowDiagonal);
        if (path != null)
        {
            foreach (var node in path)
            {
                _map[node.x, node.y] = 1; // Путь
            }
        }

        return path;
    }

    private void DrawMap()
    {
            string mapOutput = ""; // Строка для хранения всех рядов карты

            for (int y = 0; y < _height; y++)
            {
                string rowOutput = ""; // Строка для текущего ряда
                for (int x = 0; x < _width; x++)
                {
                    char cellSymbol;

                    // Определение символа для текущей клетки
                    switch (_map[x, y])
                    {
                        case 0 when x == _startPoint.x && y == _startPoint.y:
                            cellSymbol = 'O'; // Стартовая точка
                            break;
                        case 0 when x == _endPoint.x && y == _endPoint.y:
                            cellSymbol = 'X'; // Конечная точка
                            break;
                        case int n when n > 100: // Точки опасности
                            cellSymbol = 'D';
                            break;
                        case int n when n > 1: // Точки интереса
                            cellSymbol = 'I';
                            break;
                        case 1: // Ячейка пути
                            cellSymbol = 'P';
                            break;
                        default: // Пустая клетка
                            cellSymbol = '.';
                            break;
                    }

                    rowOutput += cellSymbol + " "; // Добавляем символ клетки к ряду
                }
                mapOutput += rowOutput.TrimEnd() + "\n"; // Добавляем текущий ряд в общий вывод
            }

            Debug.Log(mapOutput); // Выводим всю карту за один раз
        }
}
