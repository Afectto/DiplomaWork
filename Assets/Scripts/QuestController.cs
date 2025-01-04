using UnityEngine;
using Zenject;

public class QuestController : MonoBehaviour
{
    [SerializeField] private CodeExecutor codeExecutor;
    
    private GameStateMachine _stateMachine;
    private TaskManager _taskManager;
    private Player _player;

    [Inject]
    private void Inject(GameStateMachine stateMachine, TaskManager taskManager, Player player)
    {
        _player = player;
        _taskManager = taskManager;
        _stateMachine = stateMachine;
        _stateMachine.OnChangeState += OnChangeState;
        gameObject.SetActive(false);
    }

    private void OnChangeState(GameStateData state)
    {
        if (state == GameStateData.Quest)
        {
            gameObject.SetActive(true);
            var gridObject = _player.TileTask;
            var task = _taskManager.GetTaskForTile(new Vector2Int(gridObject.x, gridObject.y));
            codeExecutor.SetTask(task);
        }
    }
}
