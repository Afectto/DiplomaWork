using Unity.Mathematics;

public class GridObject
{
    private Grid<GridObject> _grid;
    public int x;
    public int y;
    
    public int gCost;
    public int hCost;
    public int fCost;
    public bool isWalkable;
    
    public GridObject cameFrom;
    
    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        _grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

    public float2 GetPosition()
    {
        return new float2(x, y);
    }

    public void CalculateFConst()
    {
        fCost = hCost + gCost;
    }
}
