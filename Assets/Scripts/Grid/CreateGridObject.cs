using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct GridSetting
{
    public int Width;
    public int Height;
    public float CellSize;
    public Vector3 OriginPosition;
}

public class CreateGridObject : MonoBehaviour
{
    [SerializeField] private GridSetting setting;

    private Grid<GridObject> _stringGrid;

    public Grid<GridObject> Grid => _stringGrid;

    void Awake()
    {
        _stringGrid = new Grid<GridObject>(setting,(g, x, y) => new GridObject(g,x,y));
    }
    
    private void Update()
    {
        var position = Utils.GetMouseWorldPosition();
        if (Input.GetMouseButtonDown(0))
        {
            var obj = _stringGrid.GetGridObject(position);
            if (obj != null)
            {
                Debug.Log(obj.GetPosition());
            }
        }
    }
}

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
