﻿using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager
{
    private GridManager _gridManager;
    private Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, TileData> _tileData = new Dictionary<Vector2Int, TileData>();
    private HashSet<Vector2Int> _occupiedTiles = new HashSet<Vector2Int>();
    private List<TileSprites> _tileSprites;

    public Dictionary<Vector2Int, TileData> TileDataInfo => _tileData;

    public TileManager(GridManager gridManager, List<TileSprites> tileSprites)
    {
        _tileSprites = tileSprites;
        _gridManager = gridManager;
    }

    public void InitializeTiles(GameObject tilePrefab, Transform parent)
    {
        for (int x = 0; x < _gridManager.Grid.GetWidth(); x++)
        {
            for (int y = 0; y < _gridManager.Grid.GetHeight(); y++)
            {
                Vector3 tilePosition = _gridManager.GetTileWorldPosition(new Vector2Int(x, y));
                GameObject tile = GameObject.Instantiate(tilePrefab, tilePosition, quaternion.identity, parent);
                _tiles[new Vector2Int(x, y)] = tile;
                _tileData[new Vector2Int(x, y)] = new TileData(TileType.Empty, null);
                tile.transform.localScale *= _gridManager.Grid.Setting.CellSize;
            }
        }
    }

    public void PlaceStartAndEndPoints()
    {
        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(_gridManager.Grid.GetWidth() - 1, _gridManager.Grid.GetHeight() - 1);
          
        _occupiedTiles.Add(start);
        _occupiedTiles.Add(end);

        MarkTile(start, TileType.Start);
        MarkTile(end, TileType.End);
        
        var tileEnd = _tiles[end];
        AddTextToTile(tileEnd, "End", new Vector3(0, 0, 0), 7);
    }
    
    private void AddTextToTile(GameObject tile, string text, Vector3 localPosition, float fontSize = 1f)
    {
        GameObject textObject = new GameObject("TileText");
        textObject.transform.SetParent(tile.transform);
        textObject.transform.localPosition = localPosition;

        TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
        textMeshPro.text = text;
        textMeshPro.fontSize = fontSize;
        textMeshPro.alignment = TextAlignmentOptions.Center;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = 2;
    }
    
    public void UpdateEndPointTaskData()
    {
        var userProgress = SaveSystem.Load<UserProgressInLevel>();
        var taskProgress = userProgress.GetTaskProgress();
        Vector2Int end = new Vector2Int(_gridManager.Grid.GetWidth() - 1, _gridManager.Grid.GetHeight() - 1);
        var tileEnd = _tiles[end];
        tileEnd.GetComponentInChildren<TextMeshPro>().text = taskProgress.completed + "/" + taskProgress.total;
    }

    public void PlaceDangerAndInterestPoints(int dangerPointsCount, int interestPointsCount)
    {
        PlacePoints(dangerPointsCount, TileType.Danger);
        PlacePoints(interestPointsCount, TileType.Interest);
    }

    public void GenerateWalls(int wallCount, PathfindingManager pathfinding)
    {
        List<Vector2Int> wallPositions = new List<Vector2Int>();
        var startTime = Time.realtimeSinceStartup;
        var allPaths = GetAllPaths(pathfinding);

        while (wallPositions.Count < wallCount)
        {
            Vector2Int randomPoint = GetRandomTile();

            if (IsStartOrEndPoint(randomPoint) || _occupiedTiles.Contains(randomPoint))
                continue;

            _occupiedTiles.Add(randomPoint);
            MarkTile(randomPoint, TileType.Wall);

            _tileData[randomPoint].IsWalkable = false;

            if (!IsPathsValidAfterWall(pathfinding, allPaths))
            {
                RestoreTile(randomPoint);
                _occupiedTiles.Remove(randomPoint);
            }
            else
            {
                wallPositions.Add(randomPoint);
            }
        }
        Debug.Log($"Duration Total = {((Time.realtimeSinceStartup - startTime) * 1000f).ToString("F2")} ms");

        for (int x = 0; x < _gridManager.Grid.GetWidth(); x++)
        {
            for (int y = 0; y < _gridManager.Grid.GetHeight(); y++)
            {
                var tileData = _tileData[new Vector2Int(x, y)];
                var tile = _tiles[new Vector2Int(x, y)];
                if (tileData.Type == TileType.Empty)
                {
                    GameObject.Destroy(tile.gameObject);
                    continue;
                }

                SetCollider(x, y, tileData.Type);
                var spriteRenderer = tile.GetComponentInChildren<SpriteRenderer>();
                spriteRenderer.sortingOrder = 1;
            }
        }
    }

    private void SetCollider(int x, int y, TileType tileType)
    {
        var boxCollider = _tiles[new Vector2Int(x, y)].AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = tileType != TileType.Wall;
    }
    
    private bool IsPathsValidAfterWall(PathfindingManager pathfinding, List<(Vector2Int start, Vector2Int end)> paths)
    {
        foreach (var (start, end) in paths)
        {
            var path = pathfinding.GeneratePath(new FindPathSetting
            {
                StartX = start.x,
                StartY = start.y,
                EndX = end.x,
                EndY = end.y,
                IsWalkableArray = GetWalkableArray()
            });

            if (path == null || path.Count == 0)
                return false;
        }

        return true;
    }

    private bool IsStartOrEndPoint(Vector2Int position) => 
        _tileData[position].Type == TileType.Start || _tileData[position].Type == TileType.End;

    private void RestoreTile(Vector2Int position)
    {
        _tileData[position].Type = TileType.Empty;
        _tileData[position].IsWalkable = true;
    }

    private List<(Vector2Int start, Vector2Int end)> GetAllPaths(PathfindingManager pathfinding)
    {
        List<(Vector2Int, Vector2Int)> paths = new List<(Vector2Int, Vector2Int)>
        {
            (new Vector2Int(0, 0), new Vector2Int(_gridManager.Grid.GetWidth() - 1, _gridManager.Grid.GetHeight() - 1))
        };

        foreach (var tile in _tileData)
        {
            if (tile.Value.Type == TileType.Interest || tile.Value.Type == TileType.Danger)
            {
                paths.Add((new Vector2Int(0, 0), tile.Key));
            }
        }

        return paths;
    }

    public bool[] GetWalkableArray()
    {
        bool[] walkableArray = new bool[_gridManager.Grid.GetWidth() * _gridManager.Grid.GetHeight()];

        foreach (var tile in _tileData)
        {
            int index = tile.Key.x + tile.Key.y * _gridManager.Grid.GetWidth();
            walkableArray[index] = tile.Value.IsWalkable;
        }

        return walkableArray;
    }

    private void PlacePoints(int count, TileType type)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int randomPoint = GetRandomTile();

            while (_occupiedTiles.Contains(randomPoint))
            {
                randomPoint = GetRandomTile();
            }

            _occupiedTiles.Add(randomPoint);
            MarkTile(randomPoint, type);
        }
    }

    public void MarkTile(Vector2Int position, TileType type)
    {
        if(type == TileType.Empty)
        {
            if (_tiles.ContainsKey(position))
            {
                GameObject.Destroy(_tiles[position]);
                _tiles.Remove(position);
            }
            return;
        }

        if (_tiles.ContainsKey(position))
        {
            var renderer = _tiles[position].GetComponentInChildren<SpriteRenderer>();
            if (renderer != null)
            {
                var sprite = GetSpriteForTileType(type);
                if (sprite != null)
                {
                    renderer.sprite = sprite;
                }
            }

            if (_tileData.ContainsKey(position))
            {
                _tileData[position].Type = type;
                _tileData[position].IsWalkable = type != TileType.Wall;
            }
        }
    }
    
    private Sprite GetSpriteForTileType(TileType type)
    {
        foreach (var tileSprite in _tileSprites)
        {
            if (tileSprite.type == type)
            {
                return tileSprite.Sprite;
            }
        }
        return null;
    }
    
    private Vector2Int GetRandomTile()
    {
        return new Vector2Int(
            Random.Range(1, _gridManager.Grid.GetWidth()),
            Random.Range(1, _gridManager.Grid.GetHeight()));
    }
    
    public Task GetTaskForTile(Vector2Int position)
    {
        return _tileData.ContainsKey(position) ? _tileData[position].AssignedTask : null;
    }

    public enum TileType
    {
        Empty,
        Start,
        End,
        Danger,
        Interest,
        Wall
    }

    public class TileData
    {
        public TileType Type;
        public bool IsWalkable;
        public Task AssignedTask;

        public TileData(TileType type, Task task)
        {
            Type = type;
            AssignedTask = task;
            IsWalkable = type != TileType.Wall;
        }
    }

    public void LoadLevelData(MapData levelDataMapConfiguration)
    {
        foreach (var tileData in levelDataMapConfiguration.Tiles)
        {
            Vector2Int position = tileData.Position;
            if (_tiles.ContainsKey(position))
            {
                SetCollider(position.x, position.y, tileData.Type);
                MarkTile(position, tileData.Type);
            }
        }        
        
        Vector2Int end = new Vector2Int(_gridManager.Grid.GetWidth() - 1, _gridManager.Grid.GetHeight() - 1);
        var tileEnd = _tiles[end];
        AddTextToTile(tileEnd, "End", new Vector3(0, 0, 0), 7);
        UpdateEndPointTaskData();
    }

    public bool IsEndPoint(Vector2Int vector2Int)
    {
        return _tileData.ContainsKey(vector2Int) && _tileData[vector2Int].Type == TileType.End;
    }
}