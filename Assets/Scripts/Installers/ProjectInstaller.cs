using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private GameObject sceneChanger;
    public override void InstallBindings()
    {
        BindStateMachine();
        
        Container
            .BindInterfacesAndSelfTo<UnityMainThreadDispatcher>()
            .AsSingle()
            .NonLazy();        
        
        Container
            .BindInterfacesAndSelfTo<SceneChanger>()
            .FromComponentInNewPrefab(sceneChanger)
            .AsSingle();
        
        Container
            .BindInterfacesAndSelfTo<AudioSource>()
            .FromComponentsInHierarchy()
            .AsSingle();
    }

    private void BindStateMachine()
    {
        Container.Bind<GameState>().AsSingle();
        Container.Bind<PauseState>().AsSingle();
        Container.Bind<QuestState>().AsSingle();
        Container.Bind<LoseState>().AsSingle();
        
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
