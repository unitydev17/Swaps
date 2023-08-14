using System.Collections;
using UnityEngine;
using Zenject;

public class BalloonManager : MonoBehaviour
{
    private int _counter;
    private Camera _camera;

    private Configuration _cfg;
    private ItemPool _itemPool;

    [Inject]
    public void Construct(Configuration cfg, ItemPool itemPool)
    {
        _cfg = cfg;
        _itemPool = itemPool;
    }

    public void Setup(Camera cam)
    {
        _camera = cam;
    }

    public void Activate()
    {
        StartCoroutine(SpawnBalloon());
    }

    private IEnumerator SpawnBalloon()
    {
        do
        {
            if (_counter < _cfg.maxBalloons)
            {
                var model = GetRandomModel();
                Generate(model);

                var item = _itemPool.Spawn(model);
                item.SetSortingOrder(GetSort(model));

                _counter++;
                StartCoroutine(FlyCoroutine(model, item));
            }

            yield return new WaitForSeconds(_cfg.repeatPeriod.Random());
        } while (true);

        int GetSort(BalloonModel model)
        {
            return (int) ((_cfg.scaleRange.y - model.scale) * -100);
        }
    }

    private static BalloonModel GetRandomModel()
    {
        return Random.value > 0.5f ? (BalloonModel) new OrangeBalloonModel() : new BlueBalloonModel();
    }


    private void Generate(BalloonModel model)
    {
        model.heightRange = new Vector2(_cfg.heightRange.Random(), _cfg.heightRange.Random());
        model.widthRange = Random.value > 0.5 ? new Vector2(-0.25f, 1.25f) : new Vector2(1.25f, -0.25f);
        model.amplitude = _cfg.amplitudeRange.Random();
        model.period = _cfg.periodRange.Random();
        model.speed = _cfg.speedRange.Random();
        model.scale = _cfg.scaleRange.Random();
        model.time = 0;
    }

    private IEnumerator FlyCoroutine(BalloonModel model, Balloon item)
    {
        item.transform.localScale = model.scale * Vector3.one;
        do
        {
            model.Update();
            item.transform.position = GetWorldPosition(model);

            yield return null;
        } while (model.time < 1);

        _itemPool.Despawn(item);
        _counter--;
    }

    private Vector3 GetWorldPosition(BalloonModel model)
    {
        var pos = _camera.ViewportToWorldPoint(model.pos);
        pos.z = 0;
        return pos;
    }
}