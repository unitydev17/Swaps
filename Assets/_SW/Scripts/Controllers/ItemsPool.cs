using Zenject;

public class ItemPool
{
    private WaterItem.Pool _waterItemPool;
    private FireItem.Pool _fireItemPool;

    [Inject]
    private void Construct(WaterItem.Pool waterItemPool, FireItem.Pool fireItemPool)
    {
        _waterItemPool = waterItemPool;
        _fireItemPool = fireItemPool;
    }

    public Item Spawn(ItemModel model)
    {
        return model switch
        {
            WaterModel _ => (Item) _waterItemPool.Spawn(),
            FireModel _ => _fireItemPool.Spawn(),
            _ => null
        };
    }

    public void Despawn(Item item)
    {
        if (item is WaterItem waterItem) _waterItemPool.Despawn(waterItem);
        if (item is FireItem fireItem) _fireItemPool.Despawn(fireItem);
    }
}