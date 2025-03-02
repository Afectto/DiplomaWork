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
        _playerController.Initialize(_animator, speed);
        _playerStats.OnChangeStats += ChangeStats;
    }

    private void ChangeStats(string nameStat)
    {
        if (nameStat == StatsTypeName.Health)
        {
            _healthMax = _playerStats.Stats.Health;
            Health += 0;
        }
    }

    private void Start()
    {
        _grid = _mapGenerator.GetGrid();
        transform.position = _grid.GetWorldPositionByCenterCell(0, 0);
    }

    private void FixedUpdate()
    {
        _playerController.HandleMovement();
    }

    private void Update()
    {
        if (tooltip.gameObject.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            _stateMachine.ChangeState(GameStateData.Quest);
        }
    }

    public void OnFindInterestTile(Vector3 pos)
    {
        GridObject obj = _grid.GetGridObject(pos);
        TileTask = obj;
        if (_mapGenerator.IsQuestTile(obj))
        {
            tooltip.gameObject.SetActive(true);
        }
    }

    public void OnOutFindInterestTile(Vector3 pos)
    {
        GridObject obj = _grid.GetGridObject(pos);
        TileTask = null;
        if (_mapGenerator.IsQuestTile(obj))
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    protected override void OnDeathHandler(Character owner = null)
    {
        _stateMachine.ChangeState(GameStateData.GameOver);
    }
}