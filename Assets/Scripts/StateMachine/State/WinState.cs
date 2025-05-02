using System;
using Zenject;

public class WinState : IGameState
{
    private SceneChanger _sceneChanger;

    [Inject]
    private void Inject(SceneChanger sceneChanger)
    {
        _sceneChanger = sceneChanger;
    }
    
    public void Enter()
    {
        var levelData = SaveSystem.Load<LevelData>();
        levelData.SetIsNeedSave(false);
        var levelInfo = SaveSystem.Load<LevelInfo>();
        levelInfo.SetLevel(levelInfo.LevelNumber + 1);
        _sceneChanger.ChangeScene(levelInfo.LevelNumber < levelInfo.LevelCount ? SceneNames.GameScene : SceneNames.MainMenu);
    }

    public void Exit()
    {
    }
}