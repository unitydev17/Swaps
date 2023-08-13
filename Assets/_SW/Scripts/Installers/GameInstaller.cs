using Zenject;

public class GameInstaller : MonoInstaller
{
    [Inject] private Configuration _cfg;

    public override void InstallBindings()
    {
        Container.BindFactory<BoardViewModel, BoardViewModel.Factory>().ToSelf().AsCached();
        Container.Bind<BoardManager>().ToSelf().AsSingle();
        Container.Bind<IRepository>().To<AssetStorage>().AsSingle();
        Container.BindMemoryPool<WaterItem, WaterItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[0]);
        Container.BindMemoryPool<FireItem, FireItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[1]);
        Container.Bind<ItemPool>().ToSelf().AsSingle();
        Container.Bind<NormalizeWorker>().ToSelf().AsSingle();
        Container.Bind<FlushWorker>().ToSelf().AsSingle();

        BindSignals();
    }

    private void BindSignals()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<MoveItemSignal>();
        Container.BindSignal<MoveItemSignal>().ToMethod<BoardManager>((boardManager, signal) => boardManager.ProcessMove(signal.item, signal.direction)).FromResolve();
        Container.DeclareSignal<CheckBoardSignal>();
        Container.BindSignal<CheckBoardSignal>().ToMethod<BoardManager>((boardManager, signal) => boardManager.CheckBoard()).FromResolve();
    }
}