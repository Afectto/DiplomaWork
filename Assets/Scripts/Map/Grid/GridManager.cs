using UnityEngine;

public class GridManager
{
    public Grid<GridObject> Grid { get; private set; }

    public GridManager(GridSetting settingGrid)
    {
        Grid = new Grid<GridObject>(settingGrid, (g, x, y) => new GridObject(g, x, y));
    }

    public Vector3 GetTileWorldPosition(Vector2Int position)
    {
        return GetTileWorldPosition(position.x, position.y) ;
    }
    public Vector3 GetTileWorldPosition(int x, int y)
    {
        float cellSize = Grid.GetCellSize();
        Vector3 gridOrigin = Grid.GetOriginPosition();
        return gridOrigin + new Vector3(x, y) * cellSize + new Vector3(cellSize / 2, cellSize / 2);
    }
}