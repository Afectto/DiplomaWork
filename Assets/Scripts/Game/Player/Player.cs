using TMPro;
using UnityEngine;
using Zenject;

public class Player : Character
{
    [SerializeField] private float speed;
    [SerializeField] private TextMeshPro tooltip;

    private PlayerStats _playerStats;
    private MapGenerator _mapGenerator;
    private Grid<GridObject> _grid;
    private GameStateMachine _stateMachine;
    private Animator _animator;
    private PlayerController _playerController;
    private IInteractableTile _currentInteractable;

    public StatDecorator PlayerStats => _playerStats.Stats;
    public GridObject TileTask { get; private set; }

    [Inject]
    private void Inject(MapGenerator mapGenerator, GameStateMachine stateMachine, PlayerStats playerStats)
    {
        _stateMachine = stateMachine;
        _mapGenerator = mapGenerator;
        _playerStats = playerStats;
        tooltip.gameObject.SetActive(false);
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _playerController.Initialize(_animator, _playerStats.Stats.MovementSpeed);
        Health = _playerStats.Stats.Health;
        _playerStats.OnChangeStats += ChangeStats;
    }

    private void Start()
    {
        _grid = _mapGenerator.GetGrid();
        transform.position = _grid.GetWorldPositionByCenterCell(0, 0);
        var levelData = SaveSystem.Load<LevelData>();
        if (levelData.IsNeedSave)
        {
            Health = levelData.PlayerData.HP;
            transform.position = levelData.PlayerData.Position;
        }
    }

    private void ChangeStats(string nameStat)
    {
        if (nameStat == StatsTypeName.Health)
        {
            _healthMax = _playerStats.Stats.Health;
            Health += 0;
        }

        if (nameStat == StatsTypeName.MovementSpeed)
        {
            _playerController.Initialize(_animator, _playerStats.Stats.MovementSpeed);
        }
    }
    
    private void FixedUpdate()
    {
        _playerController.HandleMovement();
    }

    private void Update()
    {
        if (_currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            _currentInteractable.OnInteract();
        }
    }

    public void OnFindInterestTile(Vector3 pos)
    {
        GridObject obj = _grid.GetGridObject(pos);
        TileTask = obj;

        _currentInteractable = null;

        if (_mapGenerator.IsQuestTile(obj))
        {
            _currentInteractable = new QuestTile(_stateMachine);
        }
        else if (_mapGenerator.IsEndPoint(obj))
        {
            var progress = SaveSystem.Load<UserProgressInLevel>();
            _currentInteractable = new EndTile(_stateMachine, progress);
        }

        if (_currentInteractable != null)
        {
            tooltip.gameObject.SetActive(true);
            tooltip.text = _currentInteractable.GetTooltipText();
        }
        else
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    public void OnOutFindInterestTile(Vector3 pos)
    {
        GridObject obj = _grid.GetGridObject(pos);
        TileTask = null;
        if (_mapGenerator.IsQuestTile(obj) || _mapGenerator.IsEndPoint(obj))
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    protected override void OnDeathHandler(Character owner = null)
    {
        _stateMachine.ChangeState(GameStateData.GameOver);
    }
}