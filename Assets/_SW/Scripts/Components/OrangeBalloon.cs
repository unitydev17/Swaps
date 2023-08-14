using Zenject;

public class OrangeBalloon : Balloon
{
    public class Pool : MemoryPool<OrangeBalloon>
    {
        protected override void OnDespawned(OrangeBalloon balloon)
        {
            balloon.gameObject.SetActive(false);
        }

        protected override void OnSpawned(OrangeBalloon balloon)
        {
            balloon.gameObject.SetActive(true);
        }
    }
}