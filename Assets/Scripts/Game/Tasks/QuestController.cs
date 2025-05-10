using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class QuestController : MonoBehaviour
{
    [SerializeField] private CodeExecutor codeExecutor;
    [SerializeField] private Button closeButton;
    
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
        codeExecutor.OnTaskComplete += OnTaskComplete;
        gameObject.SetActive(false);
        closeButton.onClick.AddListener(ClosePanel);
    }

    private async void OnTaskComplete(bool isAlreadyComplete)
    {
        await System.Threading.Tasks.Task.Delay(1000);
        ClosePanel();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
        _stateMachine.ChangeState(GameStateData.Game);
    }

    private void OnChangeState(GameStateData state)
    {
        if (state == GameStateData.Quest && _player.TileTask != null)
        {
            gameObject.SetActive(true);
            var gridObject = _player.TileTask;
            var task = _taskManager.GetTaskForTile(new Vector2Int(gridObject.x, gridObject.y));
            codeExecutor.SetTask(task);
        }
    }

    private void OnDestroy()
    {
        _stateMachine.OnChangeState -= OnChangeState;
        codeExecutor.OnTaskComplete -= OnTaskComplete;
    }
}
