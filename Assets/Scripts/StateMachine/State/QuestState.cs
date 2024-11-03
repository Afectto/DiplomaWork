using UnityEngine;

public class QuestState : IGameState
{
    public void Enter()
    {
        Time.timeScale = 0;
    }

    public void Exit()
    {
        Time.timeScale = 1;
    }
}
