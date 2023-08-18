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
                var item = _itemPool.Spawn(model);

                Generate(item, model);

                item.SetSortingOrder(GetSort(model));

                _counter++;
                StartCoroutine(FlyCoroutine(model, item));
            }

            yield return new WaitForSeconds(_cfg.repeatPeriod.Random());
        } while (true);

        int GetSort(BalloonModel model)
        {
            return (int) ((_cfg.scaleRange.y - model.scale) * -_cfg.maxBalloons);
        }
    }

    private static BalloonModel GetRandomModel()
    {
        return Random.value > 0.5f ? (BalloonModel) new OrangeBalloonModel() : new BlueBalloonModel();
    }


    private void Generate(BaseComponent item, BalloonModel model)
    {
        model.minHeight = _camera.ViewportToWorldPoint(Vector3.up * _cfg.minHeight).y;
        model.heightRange = _camera.ViewportToWorldPoint(new Vector2(_cfg.minHeight, _cfg.minHeight + Random.Range(0, 1 - _cfg.minHeight)));
        model.amplitude = _cfg.amplitudeRange.Random();
        model.period = _cfg.periodRange.Random();
        model.speed = _cfg.speedRange.Random();
        model.scale = _cfg.scaleRange.Random();
        model.widthRange = GetModelWidthRange(item, model.scale);

        model.time = 0;
    }

    private Vector2 GetModelWidthRange(BaseComponent item, float modelScale)
    {
        var sprite = item.sprite;
        var spriteExtentInUnits = sprite.texture.width / sprite.pixelsPerUnit * 0.5f;
        var screenExtentInUnits = _camera.aspect * _camera.orthographicSize;

        var leftCorner = -screenExtentInUnits;
        var rightCorner = screenExtentInUnits;


        var scaledExtentInUnits = spriteExtentInUnits * modelScale;
        var from = leftCorner - scaledExtentInUnits;
        var to = rightCorner + scaledExtentInUnits;

        return Random.value > 0.5f ? new Vector2(from, to) : new Vector2(to, from);
    }

    private IEnumerator FlyCoroutine(BalloonModel model, Component item)
    {
        item.transform.localScale = model.scale * Vector3.one;
        do
        {
            model.Update();
            item.transform.position = model.pos;

            yield return null;
        } while (model.time < 1);

        _itemPool.Despawn(item);
        _counter--;
    }
}