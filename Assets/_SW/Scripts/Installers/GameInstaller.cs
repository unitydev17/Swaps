using Zenject;

public class GameInstaller : MonoInstaller
{
    [Inject] private Configuration _cfg;
    public override void InstallBindings()
    {
        Container.Bind<LevelManager>().ToSelf().AsSingle();
        // Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IRepository>().To<AssetStorage>().AsSingle();
        Container.BindMemoryPool<WaterItem, WaterItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[0]);
        Container.BindMemoryPool<FireItem, FireItem.Pool>().FromComponentInNewPrefab(_cfg.itemPrefabs[1]);
    }
}