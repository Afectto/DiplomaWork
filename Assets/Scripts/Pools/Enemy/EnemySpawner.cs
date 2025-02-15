
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class EnemySpawner : ItemSpawners<Enemy>, IInitializable
{
    private EnemyPool _enemyItemPool;
    private Dictionary<Vector2Int, TileManager.TileData> _tileDataInfo;
    private Dictionary<Vector2Int, bool> _tileCreatedEnemy;
    private MapGenerator _mapGenerator;
    protected override ObjectPool<Enemy> ItemPool => _enemyItemPool;
    
    [Inject]
    [Obsolete("Obsolete")]
    protected override void Inject(ObjectPool<Enemy> pool)
    { 
        _enemyItemPool = pool as EnemyPool; 
        _enemyItemPool.Initialize(itemPrefab.Value as Enemy, transform);

    }

    public void Initialize()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();
        _tileDataInfo = _mapGenerator.TileManager.TileDataInfo;
        _tileCreatedEnemy = new Dictionary<Vector2Int, bool>(_tileDataInfo.Count);
        var emptyEnemyTile = _tileDataInfo.Where(tile =>
            tile.Value.Type == TileManager.TileType.Danger).ToList();
        foreach (var tile in emptyEnemyTile)
        {
            CreateItem();
        }
    }
    
    private void OnEnable()
    {
    }

    protected override void InitializeItem(IPooledObject item)
    {
        var enemy = item as Enemy;

        var emptyEnemyTile = _tileDataInfo.Where(tile =>
            tile.Value.Type == TileManager.TileType.Danger && 
            !_tileCreatedEnemy.ContainsKey(tile.Key)).ToList();
        
        var randomTile = emptyEnemyTile[Random.Range(0, emptyEnemyTile.Count)];
        _tileCreatedEnemy[randomTile.Key] = true;
        enemy.transform.position = _mapGenerator.GetGrid().GetWorldPositionByCenterCell(randomTile.Key.x, randomTile.Key.y);
    }

}
