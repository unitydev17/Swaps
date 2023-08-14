using Zenject;

public class ItemPool
{
    private WaterItem.Pool _waterItemPool;
    private FireItem.Pool _fireItemPool;
    private OrangeBalloon.Pool _orangePool;
    private BlueBalloon.Pool _bluePool;

    [Inject]
    private void Construct(WaterItem.Pool waterItemPool, FireItem.Pool fireItemPool, OrangeBalloon.Pool orangePool, BlueBalloon.Pool bluePool)
    {
        _waterItemPool = waterItemPool;
        _fireItemPool = fireItemPool;
        _orangePool = orangePool;
        _bluePool = bluePool;
    }

    public Item Spawn(ItemModel model)
    {
        return model switch
        {
            WaterModel _ => (Item) _waterItemPool.Spawn(),
            FireModel _ => (Item) _fireItemPool.Spawn(),
            _ => null
        };
    }

    public void Despawn(Item item)
    {
        if (item is WaterItem waterItem) _waterItemPool.Despawn(waterItem);
        if (item is FireItem fireItem) _fireItemPool.Despawn(fireItem);
    }

    public Balloon Spawn(BalloonModel model)
    {
        return model switch
        {
            OrangeBalloonModel _ => (Balloon) _orangePool.Spawn(),
            BlueBalloonModel _ => (Balloon) _bluePool.Spawn(),
            _ => null
        };
    }

    public void Despawn(object item)
    {
        if (item is OrangeBalloon orangeBalloon) _orangePool.Despawn(orangeBalloon);
        if (item is BlueBalloon blueBalloon) _bluePool.Despawn(blueBalloon);
    }
}