using System;
using System.Collections.Generic;

[Serializable]
public class MapData
{
    public int Width; // Ширина карты
    public int Height; // Высота карты
    public List<TileData> Tiles; // Список данных о тайлах  
    
    public MapData(string _) { }
    public MapData() { }
    
    public MapData(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new List<TileData>();
    }
}