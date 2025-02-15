using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : Character, IPooledObject
{
    [SerializeField] private float speed;
    private Player _player;
    private PathfindingManager _pathfindingManager;
    private List<int2> _path;
    private int _currentPathIndex;
    private TileManager _tileManager;
    private MapGenerator _mapGenerator;
    private Animator _animator;

    public event Action<IPooledObject> OnNeedReturnToPool;
    
    public void GetInit()
    {
    }

    public void CreateInit()
    {
        _player = FindObjectOfType<Player>();
        _mapGenerator = FindObjectOfType<MapGenerator>();
        _pathfindingManager = _mapGenerator.PathfindingManager;
        _tileManager = _mapGenerator.TileManager;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (IsPlayerInRange() && IsPathToPlayerShort())
        {
            MoveTowardsPlayer();
        }
    }

    private bool IsPlayerInRange()
    {
        var distance = Vector3.Distance(transform.position, _player.transform.position);
        return distance <= 5f;
    }
    
    private bool IsPathToPlayerShort()
    {
        var start = new int2();
        var end = new int2();
        _mapGenerator.GetGrid().GetXY(transform.position, out start.x, out start.y);
        _mapGenerator.GetGrid().GetXY(_player.transform.position, out end.x, out end.y);
        _path = _pathfindingManager.GeneratePath(new FindPathSetting {
            StartX = start.x, EndX = end.x, 
            StartY = start.y, EndY = end.y,
            IsWalkableArray = _tileManager.GetWalkableArray()});
        return (_path != null && _path.Count <= 5) || start.Equals(end);
    }

    private void MoveTowardsPlayer()
    {
        if (_path == null || _path.Count == 0) return;

        var targetPosition = _player.transform.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        SetAnimation(direction);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            _currentPathIndex++;
            if (_currentPathIndex >= _path.Count)
            {
                _path = null;        
            }
            SetAnimation(Vector3.zero);
        }
    }
    
    private void SetAnimation(Vector3 direction)
    {
        _animator.SetFloat("Horizontal", direction.x);
        _animator.SetFloat("Vertical", direction.y);
        _animator.SetFloat("Speed", direction.magnitude);
    }
    
    protected override void OnDeathHandler(Character owner = null)
    {
        TriggerOnNeedReturnToPool();
    }

    public void TriggerOnNeedReturnToPool()
    {
        OnNeedReturnToPool?.Invoke(this);
    }

}
