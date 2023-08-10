using Zenject;

public class WaterItem : Item
{
    public class Pool : MemoryPool<WaterItem>
    {
    }
}