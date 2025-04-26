using System;
using UnityEngine;

[Serializable]
public class TileData
{
    public MyVector3 Position; // Положение тайла
    public TileManager.TileType Type; // Тип тайла

    public TileData(string _) { }
    public TileData() { }
    
    public TileData(Vector2Int position, TileManager.TileType type)
    {
        Position = position;
        Type = type;
    }
}