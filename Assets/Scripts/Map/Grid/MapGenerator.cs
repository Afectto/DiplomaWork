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

    private GridManager _gridManager;
    private PathfindingManager _pathfindingManager;
    private TileManager _tileManager;
    private TaskManager _taskManager;
    
    public GridManager GridManagerInfo => _gridManager;
    public PathfindingManager PathfindingManager => _pathfindingManager;
    public TileManager TileManager => _tileManager;

    [Inject]
    private void Inject(TaskManager taskManager)
    {
        LoadLevelSettings(1);
        _gridManager = new GridManager(settingGrid);
        _pathfindingManager = new PathfindingManager(_gridManager.Grid, isAllowDiagonal);
        _tileManager = new TileManager(_gridManager);
        _taskManager = taskManager;
        
        GenerateMap();
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
    
    private void CreateBoundaryColliders()
    {
        float width = settingGrid.Width;
        float height = settingGrid.Height;
        float cellSize = settingGrid.CellSize;
        Vector3 originPosition = settingGrid.OriginPosition.ToVector3();

        // Создание коллайдеров по краям
        CreateCollider(new Vector3(width / 2, -cellSize / 2) * cellSize + originPosition, new Vector2(width * cellSize, cellSize)); // Нижний край
        CreateCollider(new Vector3(width / 2, height + cellSize / 2) * cellSize + originPosition, new Vector2(width * cellSize, cellSize)); // Верхний край
        CreateCollider(new Vector3(-cellSize / 2, height / 2) * cellSize + originPosition, new Vector2(cellSize, height * cellSize)); // Левый край
        CreateCollider(new Vector3(width + cellSize / 2, height / 2) * cellSize + originPosition, new Vector2(cellSize, height * cellSize)); // Правый край
        
    }

    private void CreateCollider(Vector2 position, Vector2 size)
    {
        GameObject colliderObject = new GameObject("BoundaryCollider");
        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.size = size * 1.5f;
        collider.transform.position = position;
        collider.transform.parent = transform;
        GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        tile.transform.localScale = new Vector3(size.x * 1.5f, size.y * 1.5f, 1);
    }
    

}
