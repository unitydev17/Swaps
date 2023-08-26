using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public class GamePool
{
    private readonly Dictionary<BaseComponent.Type, IMemoryPool<BaseComponent>> _pools;
    private readonly DiContainer _container;
    private Configuration _cfg;

    [Inject]
    public void Construct(Configuration cfg)
    {
        _cfg = cfg;
    }

    public GamePool(DiContainer container)
    {
        _container = container;
        _pools = new Dictionary<BaseComponent.Type, IMemoryPool<BaseComponent>>();
    }

    public BaseComponent Spawn(BaseComponent.Type type)
    {
        if (_pools.TryGetValue(type, out var pool))
            return pool.Spawn();

        pool = _container.Instantiate<CustomMemoryPool>(new object[]
        {
            new MultiplePoolFactory(() => _container.InstantiatePrefabForComponent<BaseComponent>(GetPrefab(type)))
        });

        _pools.Add(type, pool);
        return pool.Spawn();
    }


    private BaseComponent GetPrefab(BaseComponent.Type type)
    {
        var result = _cfg.prefabs.First(prefab => prefab.type == type);
        if (result == null) throw new Exception("Prefab type not found among the prefabs");
        return result;
    }

    public void Despawn(BaseComponent obj)
    {
        _pools[obj.type].Despawn(obj);
    }
}

public class MultiplePoolFactory : IFactory<BaseComponent>
{
    private readonly Func<BaseComponent> _factoryMethod;

    public MultiplePoolFactory(Func<BaseComponent> factoryMethod)
    {
        _factoryMethod = factoryMethod;
    }

    public BaseComponent Create()
    {
        return _factoryMethod();
    }
}