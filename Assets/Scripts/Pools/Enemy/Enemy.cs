using System;
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
        private Vector3 _startPosition;
    
        public event Action<IPooledObject> OnNeedReturnToPool;
    
        public void SetStartPosition(Vector3 pos)
        {
            _startPosition = pos;
            transform.position = pos;
        }
    
        public void GetInit() { }
    
        public void CreateInit()
        {
            _player = FindObjectOfType<Player>();
            _mapGenerator = FindObjectOfType<MapGenerator>();
            _pathfindingManager = _mapGenerator.PathfindingManager;
            _tileManager = _mapGenerator.TileManager;
            _animator = GetComponent<Animator>();
        }
    
        private bool IsTooFarFromSpawn()
        {
            return Vector3.Distance(_player.transform.position, _startPosition) > 9f && Vector3.Distance(transform.position, _startPosition) > 0.01f;
        }
    
        private void Update()
        {
            if (IsTooFarFromSpawn())
            {
                ReturnToSpawn();
            }
            else if (IsPlayerInRange() && IsPathToPlayerShort())
            {
                FollowPath();
            }
            else
            {
                SetAnimation(Vector3.zero);
            }
        }
    
        private void ReturnToSpawn()
        {
            if (_path == null || _path.Count == 0)
            {
                GeneratePathTo(_startPosition);
            }
    
            FollowPath();
        }
    
        private bool IsPlayerInRange()
        {
            return Vector3.Distance(transform.position, _player.transform.position) <= 7.5f;
        }
    
        private bool IsPathToPlayerShort()
        {
            GeneratePathTo(_player.transform.position);
            return (_path != null && _path.Count <= 7) || _path.Count == 0;
        }
    
        private void GeneratePathTo(Vector3 targetPosition)
        {
            var start = new int2();
            var end = new int2();
            _mapGenerator.GetGrid().GetXY(transform.position, out start.x, out start.y);
            _mapGenerator.GetGrid().GetXY(targetPosition, out end.x, out end.y);
            _path = _pathfindingManager.GeneratePath(new FindPathSetting
            {
                StartX = start.x,
                EndX = end.x,
                StartY = start.y,
                EndY = end.y,
                IsWalkableArray = _tileManager.GetWalkableArray()
            });
            _currentPathIndex = 0;
        }
    
        private void FollowPath()
        {
            if (_path != null && _path.Count > 0 && _currentPathIndex < _path.Count)
            {
                var targetPosition = _mapGenerator.GetGrid().GetWorldPositionByCenterCell(_path[^2].x, _path[^2].y);
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                GetComponent<FightSystem>().SetDirection(direction);
                SetAnimation(direction);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    _currentPathIndex++;
                    if (_currentPathIndex >= _path.Count)
                    {
                        _path = null;
                        SetAnimation(Vector3.zero);
                    }
                }
            }
            else
            {
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
        
        private void OnDrawGizmos()
        {
            if (_path != null && _path.Count > 0)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < _path.Count - 1; i++)
                {
                    var start = _mapGenerator.GetGrid().GetWorldPositionByCenterCell(_path[i].x, _path[i].y);
                    var end = _mapGenerator.GetGrid().GetWorldPositionByCenterCell(_path[i + 1].x, _path[i + 1].y);
                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }