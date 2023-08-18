using UnityEngine;
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
        switch (item)
        {
            case WaterItem waterItem:
                _waterItemPool.Despawn(waterItem);
                break;
            case FireItem fireItem:
                _fireItemPool.Despawn(fireItem);
                break;
        }
    }

    public BaseComponent Spawn(BalloonModel model)
    {
        return model switch
        {
            OrangeBalloonModel _ => _orangePool.Spawn(),
            BlueBalloonModel _ => _bluePool.Spawn(),
            _ => null
        };
    }

    public void Despawn(object item)
    {
        switch (item)
        {
            case OrangeBalloon orangeBalloon:
                _orangePool.Despawn(orangeBalloon);
                break;
            case BlueBalloon blueBalloon:
                _bluePool.Despawn(blueBalloon);
                break;
        }
    }
}