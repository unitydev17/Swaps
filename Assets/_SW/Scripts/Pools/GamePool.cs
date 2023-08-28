using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GamePool
{
    private readonly Dictionary<BaseComponent.Type, IMemoryPool<BaseComponent>> _pools;
    private readonly DiContainer _container;
    private Configuration _cfg;
    private Transform _rootTr;

    [Inject]
    public void Construct(Configuration cfg)
    {
        _cfg = cfg;
        _rootTr = new GameObject(typeof(GamePool).ToString()).transform;
    }

    public GamePool(DiContainer container)
    {
        _container = container;
        _pools = new Dictionary<BaseComponent.Type, IMemoryPool<BaseComponent>>();
    }

    public BaseComponent Spawn(BaseComponent.Type type)
    {
        if (_pools.TryGetValue(type, out var pool))
        {
            return Spawned();
        }

        pool = _container.Instantiate<CustomMemoryPool>(new object[]
        {
            new MultiplePoolFactory(() => _container.InstantiatePrefabForComponent<BaseComponent>(GetPrefab(type)))
        });

        _pools.Add(type, pool);
        return Spawned();

        BaseComponent Spawned()
        {
            var obj = pool.Spawn();
            var transform = obj.transform;
            transform.SetParent(_rootTr);
            transform.localScale = Vector3.one;
            return obj;
        }
    }


    private BaseComponent GetPrefab(BaseComponent.Type type)
    {
        return _cfg.prefabs.First(prefab => prefab.type == type);
    }

    public void Despawn(BaseComponent obj)
    {
        obj.transform.parent = _rootTr;
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