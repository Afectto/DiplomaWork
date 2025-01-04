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

    [Inject]
    private void Inject(TaskManager taskManager)
    {
        _gridManager = new GridManager(settingGrid);
        _pathfindingManager = new PathfindingManager(_gridManager.Grid, isAllowDiagonal);
        _tileManager = new TileManager(_gridManager);
        _taskManager = taskManager;

        GenerateMap();
    }

    private void GenerateMap()
    {
        _tileManager.InitializeTiles(tilePrefab, transform);
        _tileManager.PlaceStartAndEndPoints();
        _tileManager.PlaceDangerAndInterestPoints(dangerPointsCount, interestPointsCount);
        _tileManager.GenerateWalls(wallCount, _pathfindingManager);
        _taskManager.GenerateTasks(_tileManager.TileDataInfo, (minTasksDifficulty, maxTasksDifficulty));
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
}
