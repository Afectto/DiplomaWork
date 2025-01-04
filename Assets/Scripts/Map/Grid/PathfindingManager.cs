using System.Collections.Generic;
using Unity.Mathematics;

public class PathfindingManager
{
    private Grid<GridObject> _grid;
    private bool _isAllowDiagonal;

    public PathfindingManager(Grid<GridObject> grid, bool isAllowDiagonal)
    {
        _grid = grid;
        _isAllowDiagonal = isAllowDiagonal;
    }

    public List<int2> GeneratePath(FindPathSetting settings)
    {
        var pathfinding = new PathfindingJob(_grid);
        return pathfinding.FindPath(settings, _isAllowDiagonal);
    }
}