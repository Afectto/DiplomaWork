using UnityEngine;

public class EndTile : IInteractableTile
{
    private readonly GameStateMachine _stateMachine;
    private readonly UserProgressInLevel _progress;

    public EndTile(GameStateMachine stateMachine, UserProgressInLevel progress)
    {
        _stateMachine = stateMachine;
        _progress = progress;
    }

    public void OnInteract()
    {
        var taskProgress = _progress.GetTaskProgress();
        if (taskProgress.completed == taskProgress.total)
            _stateMachine.ChangeState(GameStateData.Win);
        else
            Debug.Log("Нельзя перейти — задания не выполнены");
    }

    public string GetTooltipText()
    {
        var taskProgress = _progress.GetTaskProgress();
        return taskProgress.completed == taskProgress.total
            ? "Tap E to next level"
            : "You need to complete all tasks";
    }
}