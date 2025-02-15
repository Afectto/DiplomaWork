using UnityEngine;

public class LoseState : IGameState
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