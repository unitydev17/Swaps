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
        Container.Bind<BoardController>().ToSelf().AsSingle();
        Container.Bind<BalloonManager>().FromComponentInHierarchy().AsSingle();

        BindRepositories();
        BindSignals();
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

    private void BindSignals()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<MoveItemSignal>();
        Container.BindSignal<MoveItemSignal>().ToMethod<BoardController>((boardManager, signal) => boardManager.ProcessMove(signal.item, signal.direction)).FromResolve();
        Container.DeclareSignal<ValidateBoard>();
        Container.BindSignal<ValidateBoard>().ToMethod<BoardController>((boardManager, signal) => boardManager.ValidateAndProcessBoard()).FromResolve();
        Container.DeclareSignal<LevelCompletedSignal>();
        Container.BindSignal<LevelCompletedSignal>().ToMethod<GameManager>((gameManager, signal) => gameManager.LevelCompleted()).FromResolve();
        Container.DeclareSignal<ForceNextLevelSignal>();
        Container.BindSignal<ForceNextLevelSignal>().ToMethod<GameManager>((gameManager, signal) => gameManager.ForceNextLevel()).FromResolve();
        Container.DeclareSignal<RetryLevelSignal>();
        Container.BindSignal<RetryLevelSignal>().ToMethod<GameManager>((gameManager, signal) => gameManager.RetryLevel()).FromResolve();
        Container.DeclareSignal<RetryButtonRequiredSignal>();
        Container.BindSignal<RetryButtonRequiredSignal>().ToMethod<UiController>((uiController, signal) => uiController.AppearRetryButton()).FromResolve();
    }
}