using Zenject;

public class GameInstaller : MonoInstaller
{
    [Inject] private Configuration _cfg;

    public override void InstallBindings()
    {
        Container.BindFactory<BoardViewModel, BoardViewModel.Factory>().ToSelf().AsCached();

        Container.Bind<ItemPool>().ToSelf().AsSingle();
        Container.Bind<NormalizeWorker>().ToSelf().AsSingle();
        Container.Bind<FlushWorker>().ToSelf().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();

        BindRepositories();
        BindSignals();
        BindPools();
    }

    private void BindPools()
    {
        Container.Bind<BoardController>().ToSelf().AsSingle();
        Container.BindMemoryPool<WaterItem, WaterItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[0]);
        Container.BindMemoryPool<FireItem, FireItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[1]);
    }

    private void BindRepositories()
    {
        Container.Bind<IRepository>().To<LevelStorage>().AsSingle();
        Container.Bind<IUserRepository>().To<UserDataStorage>().AsSingle();
    }

    private void BindSignals()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<MoveItemSignal>();
        Container.BindSignal<MoveItemSignal>().ToMethod<BoardController>((boardManager, signal) => boardManager.ProcessMove(signal.item, signal.direction)).FromResolve();
        Container.DeclareSignal<ValidateBoard>();
        Container.BindSignal<ValidateBoard>().ToMethod<BoardController>((boardManager, signal) => boardManager.ValidateAndProcessBoard()).FromResolve();
        Container.DeclareSignal<LevelCompletedSignal>();
        Container.BindSignal<LevelCompletedSignal>().ToMethod<GameManager>((gameManager, signal) => gameManager.LevelCompleted()).FromResolve();
    }
}