using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameController : MonoBehaviour
{
    private EnemySpawner _enemySpawner;
    private Player _player;
    private GameStateMachine _stateMachine;

    [Inject]
    private void Inject(EnemySpawner enemySpawner, GameStateMachine stateMachine, Player player)
    {
        _player = player;
        _stateMachine = stateMachine;
        _stateMachine.ChangeState(GameStateData.Game);
        _stateMachine.OnChangeState += OnChangeState;
        _enemySpawner = enemySpawner;
    }

    private void OnChangeState(GameStateData state)
    {
        switch (state)
        {
            case GameStateData.Pause:
                SaveGame(true);
                break;
            case GameStateData.GameOver:
                var completedTasks = SaveSystem.Load<UserProgressInLevel>();
                completedTasks.SetData(new List<Task>());//Обнуляем все задачи при проигрыше
                SaveGame(false);
                break;
        }
    }

    private void SaveGame(bool isNeedSave)
    {
        var levelData = SaveSystem.Load<LevelData>();
        levelData.SetIsNeedSave(isNeedSave);
        if (!isNeedSave)return;
        
        var listEnemy = _enemySpawner.GetActiveObjects();
        List<EnemyData> enemyData = new List<EnemyData>();
        foreach (var enemy in listEnemy)
        {
            enemyData.Add(new EnemyData(enemy));
        }
        
        var mapGenerator = FindObjectOfType<MapGenerator>();
        var tileData = mapGenerator.GetMapData();
        var taskData = mapGenerator.GetTaskData();
        
        levelData.SetData(enemyData, tileData, taskData, new PlayerData(_player));
        levelData.SetLevel(1);
    }
    
    private void OnApplicationQuit()
    {
        // SaveGame(true);
    }
}
