using Zenject;

public class WaterItem : Item
{
    public class Pool : MemoryPool<WaterItem>
    {
        protected override void OnDespawned(WaterItem item)
        {
            item.Deactivate();
        }

        protected override void OnSpawned(WaterItem item)
        {
            item.Activate();
        }
    }
}