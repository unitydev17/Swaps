using Zenject;

public class GameInstaller : MonoInstaller
{
    [Inject] private Configuration _cfg;

    public override void InstallBindings()
    {
        Container.Bind<AppModel>().ToSelf().AsSingle();
        Container.Bind<ItemPool>().ToSelf().AsSingle();
        Container.Bind<NormalizeWorker>().ToSelf().AsSingle();
        Container.Bind<FlushWorker>().ToSelf().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UiController>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();
        Container.Bind<BalloonManager>().FromComponentInHierarchy().AsSingle();
        
        BindRepositories();
        BindPools();
        BindFactories();
    }

    private void BindFactories()
    {
        Container.BindFactory<BoardViewModel, BoardViewModel.Factory>().ToSelf().AsCached();
    }

    private void BindPools()
    {
        Container.BindMemoryPool<WaterItem, WaterItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[0]);
        Container.BindMemoryPool<FireItem, FireItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[1]);
        Container.BindMemoryPool<OrangeBalloon, OrangeBalloon.Pool>().FromComponentInNewPrefab(_cfg.balloonPrefabs[0]);
        Container.BindMemoryPool<BlueBalloon, BlueBalloon.Pool>().FromComponentInNewPrefab(_cfg.balloonPrefabs[1]);
    }

    private void BindRepositories()
    {
        Container.Bind<IRepository>().To<LevelStorage>().AsSingle();
        Container.Bind<IUserRepository>().To<UserDataStorage>().AsSingle();
    }
}