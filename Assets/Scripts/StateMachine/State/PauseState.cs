using System;
using UnityEngine;

public class PauseState : IGameState
{
    public Action OnExitPause;
    public void Enter()
    {
        Time.timeScale = 0;
    }

    public void Exit()
    {
        Time.timeScale = 1;
        OnExitPause?.Invoke();
    }
}