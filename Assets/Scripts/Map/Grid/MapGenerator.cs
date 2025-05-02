using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GridSetting settingGrid;
    [SerializeField] private int dangerPointsCount = 3;
    [SerializeField] private int interestPointsCount = 3;
    [SerializeField] private bool isAllowDiagonal;
    [SerializeField] private int wallCount;
    [SerializeField, Min(1)] private int minTasksDifficulty;
    [SerializeField, Min(1)] private int maxTasksDifficulty;

    [SerializeField] private List<TileSprites> tileSprites;

    private GridManager _gridManager;
    private PathfindingManager _pathfindingManager;
    private TileManager _tileManager;
    private TaskManager _taskManager;
    private MapData _mapData;

    public GridManager GridManagerInfo => _gridManager;
    public PathfindingManager PathfindingManager => _pathfindingManager;
    public TileManager TileManager => _tileManager;

    [Inject]
    private void Inject(TaskManager taskManager)
    {
        var levelData = SaveSystem.Load<LevelData>();
        if (levelData.IsNeedSave)
        {
            LoadLevelSettings(levelData.Level);
        }
        else
        {
            var levelInfo = SaveSystem.Load<LevelInfo>();
            LoadLevelSettings(levelInfo.LevelNumber);
        }
        _gridManager = new GridManager(settingGrid);
        _pathfindingManager = new PathfindingManager(_gridManager.Grid, isAllowDiagonal);
        _tileManager = new TileManager(_gridManager, tileSprites);
        _taskManager = taskManager;

        if (levelData.IsNeedSave)
        {
            LoadLevel(levelData);
        }
        else
        {
            var completedTasks = SaveSystem.Load<UserProgressInLevel>();
            completedTasks.SetData(new List<Task>());//Обнуляем все задачи при генерации уровня
            GenerateMap();
        }
    }

    private void LoadLevel(LevelData levelData)
    {
        _tileManager.InitializeTiles(tilePrefab, transform);
        _tileManager.LoadLevelData(levelData.MapConfiguration);
        _taskManager.LoadLevelData(levelData.Tasks);
        CreateBoundaryColliders();
    }

    private void LoadLevelSettings(int level)
    {
        var saveData = SaveSystem.Load<SaveLevelData>();
        var levelSettings = saveData.LevelSettingsDictionary[level];
        settingGrid = levelSettings.SettingGrid;
        dangerPointsCount = levelSettings.DangerPointsCount;
        interestPointsCount = levelSettings.InterestPointsCount;
        wallCount = levelSettings.WallCount;
        minTasksDifficulty = levelSettings.MinTasksDifficulty;
        maxTasksDifficulty = levelSettings.MaxTasksDifficulty;
    }

    private void GenerateMap()
    {
        _tileManager.InitializeTiles(tilePrefab, transform);
        _tileManager.PlaceStartAndEndPoints();
        _tileManager.PlaceDangerAndInterestPoints(dangerPointsCount, interestPointsCount);
        _tileManager.GenerateWalls(wallCount, _pathfindingManager);
        _taskManager.GenerateTasks(_tileManager.TileDataInfo, (minTasksDifficulty, maxTasksDifficulty));
        CreateBoundaryColliders();
        SetTileData();
        UpdateEndPointTaskData();
    }

    public void UpdateEndPointTaskData()
    {
        _tileManager.UpdateEndPointTaskData();
    }
    
    public List<TaskData> GetTaskData()
    {
        return _taskManager.GetTaskData();
    }
    
    private void SetTileData()
    {
        List<TileData> tileDataList = new List<TileData>();
        
        foreach (var tile in _tileManager.TileDataInfo)
        {
            tileDataList.Add(new TileData(tile.Key, tile.Value.Type));
        }
        _mapData = new MapData(settingGrid.Width, settingGrid.Height);
        _mapData.Tiles = tileDataList;
    }

    public MapData GetMapData()
    {
        return _mapData;
    }

    public Task GetTaskForTile(Vector2Int position)
    {
        return _taskManager.GetTaskForTile(position);
    }

    public Grid<GridObject> GetGrid()
    {
        return _gridManager.Grid;
    }

    public bool IsQuestTile(GridObject cell)
    {
        return GetTaskForTile(new Vector2Int(cell.x, cell.y)) != null;
    }
    
    public bool IsEndPoint(GridObject cell)
    {
        return _tileManager.IsEndPoint(new Vector2Int(cell.x, cell.y));
    }
    
    private void CreateBoundaryColliders()
    {
        float width = settingGrid.Width;
        float height = settingGrid.Height;
        float cellSize = settingGrid.CellSize;
        Vector3 originPosition = settingGrid.OriginPosition;

        // Смещение на пол тайла
        Vector3 offset = new Vector3(cellSize / 2, cellSize / 2, 0);

        // Создание тайлов для нижнего и верхнего края
        for (int x = -1; x <= width; x++) // Увеличиваем диапазон, чтобы покрыть края
        {
            Vector3 bottomPosition = originPosition + new Vector3(x * cellSize, -1 * cellSize, 0) + offset;
            Vector3 topPosition = originPosition + new Vector3(x * cellSize, height * cellSize, 0) + offset;
            CreateTile(bottomPosition);
            CreateTile(topPosition);
        }

        // Создание тайлов для левого и правого края
        for (int y = 0; y < height; y++)
        {
            Vector3 leftPosition = originPosition + new Vector3(-1 * cellSize, y * cellSize, 0) + offset;
            Vector3 rightPosition = originPosition + new Vector3(width * cellSize, y * cellSize, 0) + offset;
            CreateTile(leftPosition);
            CreateTile(rightPosition);
        }

        // Заполнение углов
        CreateTile(originPosition + new Vector3(-1 * cellSize, -1 * cellSize, 0) + offset); // Левый нижний угол
        CreateTile(originPosition + new Vector3(-1 * cellSize, height * cellSize, 0) + offset); // Левый верхний угол
        CreateTile(originPosition + new Vector3(width * cellSize, -1 * cellSize, 0) + offset); // Правый нижний угол
        CreateTile(originPosition + new Vector3(width * cellSize, height * cellSize, 0) + offset); // Правый верхний угол
    }

    private void CreateTile(Vector3 position)
    {
        GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        tile.transform.localScale = new Vector3(settingGrid.CellSize, settingGrid.CellSize, 1);
        tile.name = "BoundaryTile";
        var collider = tile.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
    }

}