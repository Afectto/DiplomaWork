using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<MapGenerator>()
            .FromComponentsInHierarchy()
            .AsSingle();
        
        Container
            .BindInterfacesAndSelfTo<QuestController>()
            .FromComponentsInHierarchy()
            .AsSingle();

        Container
            .BindInterfacesAndSelfTo<TaskManager>()
            .AsSingle()
            .NonLazy();
        
        Enemy();
        PlayerInit();
    }

    private void PlayerInit()
    {
        Container
            .BindInterfacesAndSelfTo<Player>()
            .FromComponentInHierarchy()
            .AsSingle();
        
        Container
            .BindInterfacesAndSelfTo<PlayerStats>()
            .AsSingle()
            .NonLazy(); 
    }


    private void Enemy()
    {
        Container
            .Bind<ObjectPool<Enemy>>()
            .To<EnemyPool>()
            .AsSingle();

        Container
            .BindInterfacesAndSelfTo<EnemySpawner>()
            .FromComponentsInHierarchy()
            .AsSingle();
    }
}
