using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AppModel>().ToSelf().AsSingle();
        Container.Bind<NormalizeWorker>().ToSelf().AsSingle();
        Container.Bind<IFlushWorker>().To<RecursiveFlushWorker>().AsSingle();
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
        Container.Bind<GamePool>().ToSelf().AsSingle();
    }

    private void BindRepositories()
    {
        Container.Bind<IRepository>().To<LevelStorage>().AsSingle();
        Container.Bind<IUserRepository>().To<UserDataStorage>().AsSingle();
    }
}