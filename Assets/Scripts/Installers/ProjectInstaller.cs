using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindStateMachine();
    }

    private void BindStateMachine()
    {
        Container.Bind<GameState>().AsSingle();
        Container.Bind<PauseState>().AsSingle();
        Container.Bind<QuestState>().AsSingle();
        
        Container
            .BindInterfacesAndSelfTo<GameStateMachine>()
            .AsSingle()
            .OnInstantiated((context, instance) =>
            {
                if (instance is GameStateMachine gameStateMachine)
                {
                    gameStateMachine.ChangeState(GameStateData.Game);
                }
            })
            .NonLazy();
    }
}
