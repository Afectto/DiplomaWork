using System;
using UnityEngine;

public class Grid<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }
    
    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _originPosition;

    private TextMesh[,] debugTexArray;
    private TGridObject[,] _gridArray;
    public readonly GridSetting Setting;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject> , int, int, TGridObject> createGridObject = null)
    {

        Initialize(width, height, cellSize, originPosition, createGridObject);
    }

    public Grid(GridSetting setting, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject = null)
    {
        Setting = setting;
        Initialize(setting.Width, setting.Height, setting.CellSize, setting.OriginPosition, createGridObject);
    }
    
    private void Initialize(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject> , int, int, TGridObject> createGridObject = null)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _originPosition = originPosition;

        _gridArray = new TGridObject[width, height];
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _gridArray.GetLength(1); y++)
            {
                if (createGridObject != null)
                    _gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        bool isDebug = true;
        if (isDebug)
        {
            ShowDebug(width, height);
        }
    }
    
    private void ShowDebug(int width, int height)
    {
        debugTexArray = new TextMesh[width, height];
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _gridArray.GetLength(1); y++)
            {
                // debugTexArray[x, y] = Utils.CreateWorldText(_gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(_cellSize, _cellSize) * .5f, 2, Color.white, TextAnchor.MiddleCenter);
                
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
        {
            // debugTexArray[eventArgs.x, eventArgs.y].text = _gridArray[eventArgs.x, eventArgs.y]?.ToString();
        };
    }

    public int GetWidth()
    {
        return _width;
    }
    
    public int GetHeight()
    {
        return _height;
    }

    public float GetCellSize()
    {
        return _cellSize;
    }

    public Vector3 GetOriginPosition()
    {
        return _originPosition;
    }
    
    public Vector3 GetWorldPositionByCenterCell(int x, int y)
    {
        Vector3 pos = new Vector3(x, y, 0) * _cellSize;

        pos.x += _cellSize * 0.5f;
        pos.y += _cellSize * 0.5f;

        pos += _originPosition;
        pos.z = 0;

        return pos;
    }
    
    public Vector3 GetWorldPosition(int x, int y)
    {
        var pos = new Vector3(x, y, 0) * _cellSize + _originPosition;
        pos.z = 0;
        return pos;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
        y = Mathf.FloorToInt((worldPosition - _originPosition).y / _cellSize);
    }

    private void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            _gridArray[x, y] = value;
        }
    }

    public void TriggerGridObjectChange(int x, int y)
    {
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs{x= x, y = y});
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }
    
    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            return _gridArray[x, y];
        }

        return default(TGridObject);
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }
}
