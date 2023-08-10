using Zenject;

public class FireItem : Item
{
    public class Pool : MemoryPool<FireItem>
    {
    }
}