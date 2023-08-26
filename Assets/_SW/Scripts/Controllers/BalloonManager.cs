using System.Collections;
using UnityEngine;
using Zenject;

public class BalloonManager : MonoBehaviour
{
    private int _counter;
    private Camera _camera;

    private Configuration _cfg;
    private GamePool _itemPool;
    private AppModel _appModel;

    [Inject]
    public void Construct(Configuration cfg, GamePool itemPool, AppModel appModel)
    {
        _cfg = cfg;
        _itemPool = itemPool;
        _appModel = appModel;
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
                var item = (Balloon) _itemPool.Spawn(GetRandomBalloonType());
                item.model = GenerateModel(item);
                item.SetSortingOrder(GetSort(item));

                _counter++;
                StartCoroutine(FlyCoroutine(item));
            }

            yield return new WaitForSeconds(_cfg.repeatPeriod.Random());
        } while (_appModel.gameActive);

        int GetSort(Balloon item)
        {
            return (int) ((_cfg.scaleRange.y - item.model.scale) * -_cfg.maxBalloons);
        }
    }

    private static BaseComponent.Type GetRandomBalloonType()
    {
        return Random.value > 0.5f ? BaseComponent.Type.BlueBalloon : BaseComponent.Type.OrangeBalloon;
    }


    private BalloonModel GenerateModel(BaseComponent item)
    {
        var model = new BalloonModel
        {
            minHeight = _camera.ViewportToWorldPoint(Vector3.up * _cfg.minHeight).y,
            heightRange = _camera.ViewportToWorldPoint(new Vector2(_cfg.minHeight, _cfg.minHeight + Random.Range(0, 1 - _cfg.minHeight))),
            amplitude = _cfg.amplitudeRange.Random(),
            period = _cfg.periodRange.Random(),
            speed = _cfg.speedRange.Random(),
            scale = _cfg.scaleRange.Random(),
            time = 0
        };
        model.widthRange = GetModelWidthRange(item, model.scale);
        return model;
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

    private IEnumerator FlyCoroutine(Balloon item)
    {
        item.transform.localScale = item.model.scale * Vector3.one;
        do
        {
            item.model.Update();
            item.transform.position = item.model.pos;

            yield return null;
        } while (item.model.time < 1);

        _itemPool.Despawn(item);
        _counter--;
    }
}