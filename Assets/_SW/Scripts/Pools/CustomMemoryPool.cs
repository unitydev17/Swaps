using Zenject;

public class CustomMemoryPool : MemoryPool<BaseComponent>
{
    protected override void OnSpawned(BaseComponent item)
    {
        item.OnSpawned();
    }

    protected override void OnDespawned(BaseComponent item)
    {
        item.OnDespawned();
    }
}