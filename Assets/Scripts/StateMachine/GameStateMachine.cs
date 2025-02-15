using System;
using System.Collections.Generic;
using System.Linq;

public class GameStateMachine
{
    private readonly Dictionary<GameStateData, IGameState> _states;
    private readonly Dictionary<IGameState, GameStateData> _reverseStates;
    private IGameState _currentState;

    public event Action<GameStateData> OnChangeState;

    public GameStateMachine(GameState gameState, PauseState pauseState, QuestState questState, LoseState loseState)
    {
        
        _states = new Dictionary<GameStateData, IGameState>
        {
            { GameStateData.Game, gameState },
            { GameStateData.Pause, pauseState },
            { GameStateData.Quest, questState },
            { GameStateData.GameOver, loseState }
        };
        _reverseStates = _states.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    }

    public void ChangeState(GameStateData newStateData)
    {
        if (_currentState == null)
        {
            _currentState = _states[newStateData];
            OnChangeState?.Invoke(_reverseStates[_currentState]);
            _currentState.Enter();
        }
        if(_reverseStates[_currentState] == newStateData) return;
        
        _currentState?.Exit();
        _currentState = _states[newStateData];
        OnChangeState?.Invoke(_reverseStates[_currentState]);
        _currentState.Enter();
    }

    public GameStateData GetCurrentState()
    {
        return _reverseStates[_currentState];
    }

}