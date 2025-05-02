using System;

[Serializable]
public struct GridSetting
{
    public int Width;
    public int Height;
    public float CellSize;
    public MyVector3 OriginPosition;
    
    public GridSetting(int width, int height, float cellSize, MyVector3 originPosition)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        OriginPosition = originPosition;
    }
}