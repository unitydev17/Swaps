using Zenject;

public class BlueBalloon : Balloon
{
    public class Pool : MemoryPool<BlueBalloon>
    {
        protected override void OnDespawned(BlueBalloon balloon)
        {
            balloon.gameObject.SetActive(false);
        }

        protected override void OnSpawned(BlueBalloon balloon)
        {
            balloon.gameObject.SetActive(true);
        }
    }
}