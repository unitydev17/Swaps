using Zenject;

public class GameInstaller : MonoInstaller
{
    [Inject] private Configuration _cfg;

    public override void InstallBindings()
    {
        Container.BindFactory<BoardViewModel, BoardViewModel.Factory>().ToSelf().AsCached();
        Container.Bind<LevelManager>().ToSelf().AsSingle();
        Container.Bind<IRepository>().To<AssetStorage>().AsSingle();
        Container.BindMemoryPool<WaterItem, WaterItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[0]);
        Container.BindMemoryPool<FireItem, FireItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[1]);
        Container.Bind<ItemPool>().ToSelf().AsSingle();
        BindSignals();
    }

    private void BindSignals()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<MoveItemSignal>();
        Container.BindSignal<MoveItemSignal>().ToMethod<LevelManager>((levelManager, signal) => levelManager.ProcessMove(signal.item, signal.direction)).FromResolve();
    }
}