using Zenject;

public class FireItem : Item
{
    public class Pool : MemoryPool<FireItem>
    {
        protected override void OnDespawned(FireItem item)
        {
            item.Deactivate();
        }

        protected override void OnSpawned(FireItem item)
        {
            item.Activate();
        }
    }
}