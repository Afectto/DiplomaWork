public class QuestTile : IInteractableTile
{
    private readonly GameStateMachine _stateMachine;

    public QuestTile(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void OnInteract()
    {
        _stateMachine.ChangeState(GameStateData.Quest);
    }

    public string GetTooltipText() => "Tap E to activate";
}