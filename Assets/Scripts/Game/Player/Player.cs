using TMPro;
using UnityEngine;
using Zenject;

public class Player : Character
{
    [SerializeField] private float speed;
    [SerializeField] private TextMeshPro tooltip;

    private MapGenerator _mapGenerator;
    private Grid<GridObject> _grid;
    private GameStateMachine _stateMachine;
    private Animator _animator;
    
    public GridObject TileTask { get; private set; }

    [Inject]
    private void Inject(MapGenerator mapGenerator, GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _mapGenerator = mapGenerator; 
        tooltip.gameObject.SetActive(false);
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _grid = _mapGenerator.GetGrid();
        transform.position = _grid.GetWorldPositionByCenterCell(0, 0);
    }

    private void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        _animator.SetFloat("Horizontal", moveX);
        _animator.SetFloat("Vertical", moveY);
        _animator.SetFloat("Speed", Mathf.Abs(moveX) + Mathf.Abs(moveY));
        
        Vector3 newPosition = transform.localPosition + new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;

        if (CanPlayerEnterCell(newPosition))
        {
            transform.localPosition = newPosition;
        }
    }

    private void Update()
    {
        if (tooltip.gameObject.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            _stateMachine.ChangeState(GameStateData.Quest);
        }
    }

    private bool CanPlayerEnterCell(Vector3 worldPosition)
    {
        var offsetPosition = worldPosition;
        int x, y;
        _grid.GetXY(offsetPosition, out x, out y);
        var cell = _grid.GetGridObject(x, y);
        return cell != null && cell.isWalkable;
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