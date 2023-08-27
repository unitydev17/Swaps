using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BoardController : IInitializable, IDisposable
{
    public static event Action OnLevelCompleted;
    public static event Action OnImpossibleToComplete;


    private BoardViewModel _boardViewModel;
    private Board _board;
    private Transform _pivot;

    private Configuration _cfg;
    private BoardViewModel.Factory _boardViewModelFactory;
    private NormalizeWorker _normalizeWorker;
    private FlushWorker _flushWorker;
    private AppModel _appModel;
    private Camera _camera;

    private GamePool _commonPool;


    [Inject]
    private void Construct(
        AppModel appModel,
        Configuration cfg,
        BoardViewModel.Factory boardViewModelFactory,
        NormalizeWorker normalizeWorker,
        FlushWorker flushWorker,
        GamePool commonPool)
    {
        _appModel = appModel;
        _cfg = cfg;
        _boardViewModelFactory = boardViewModelFactory;
        _normalizeWorker = normalizeWorker;
        _flushWorker = flushWorker;
        _commonPool = commonPool;
    }


    public void Initialize()
    {
        InputController.OnMove += ProcessMove;
    }

    public void Dispose()
    {
        InputController.OnMove -= ProcessMove;
    }


    public void SetBoard(Board board, Transform pivot, Camera camera)
    {
        _board = board;
        _pivot = pivot;
        _camera = camera;
    }

    public void Activate()
    {
        CreateBoard();
    }

    private void CreateBoard()
    {
        ResetRootScale();
        AlignPivot();

        var items = CreateItemsFromModel();
        _boardViewModel = _boardViewModelFactory.Create();
        _boardViewModel.Init(items, _board);

        CorrectRootScale();
    }

    private void ResetRootScale()
    {
        _pivot.parent.localScale = Vector3.one;
    }

    private void AlignPivot()
    {
        var pivotOffsetX = (_board.width - 1) * _cfg.offset.x * 0.5f;
        _pivot.localPosition = Vector3.left * pivotOffsetX;
    }

    private void CorrectRootScale()
    {
        var spriteInUnits = _cfg.sprite.texture.width / _cfg.sprite.pixelsPerUnit;
        var scale = _camera.aspect * _camera.orthographicSize * 2 / (_board.width * spriteInUnits * _cfg.spriteTransparencyKoeff);
        scale = Mathf.Min(scale, _cfg.maxSpriteScale);
        _pivot.parent.localScale = Vector3.one * scale;
    }

    private List<Item> CreateItemsFromModel()
    {
        var items = new List<Item>();

        for (var i = 0; i < _board.items.Count; i++)
        {
            var itemModel = _board.items[i];
            if (itemModel is EmptyModel) continue;

            var item = CreateItem(itemModel, i);
            items.Add(item);
        }

        return items;
    }

    private Item CreateItem(ItemModel itemModel, int i)
    {
        Item item;

        if (itemModel is FireModel)
        {
            item = (Item) _commonPool.Spawn(BaseComponent.Type.Fire);
        }
        else
        {
            item = (Item) _commonPool.Spawn(BaseComponent.Type.Water);
        }


        item.transform.parent = _pivot;
        item.index = i;

        var pos = _board.GetPos(i);
        item.transform.localPosition = new Vector3(pos.x * _cfg.offset.x, pos.y * _cfg.offset.y, 0);

        return item;
    }


    private void ProcessMove(Item item, Vector2Int direction)
    {
        if (_appModel.inputDenied) return;
        _appModel.inputDenied = true;

        var currIndex = item.index;
        var nextPos = _board.GetPos(currIndex) + direction;
        var nextIndex = _board.GetIndex(nextPos);

        if (!CheckBounds(nextPos))
        {
            _appModel.inputDenied = false;
            return;
        }

        if (CheckEmpty(nextPos))
        {
            if (IsMoveUp(direction))
            {
                _appModel.inputDenied = false;
                return;
            }

            _board.Move(currIndex, nextIndex);
            _boardViewModel.AnimateMove(currIndex, nextIndex, _board.GetPos(nextIndex), async () => await ValidateAndProcessBoard());
            return;
        }

        _board.Swap(currIndex, nextIndex);
        _boardViewModel.AnimateSwap(currIndex, nextIndex, _board.GetPos(currIndex), _board.GetPos(nextIndex), async () => await ValidateAndProcessBoard());
    }

    private static bool IsMoveUp(Vector2Int direction)
    {
        return direction.y > 0;
    }

    private bool CheckEmpty(Vector2Int nextPos)
    {
        return _board.GetItemModel(nextPos) is EmptyModel;
    }


    private bool CheckBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < _board.width && pos.y >= 0 && pos.y < _board.height;
    }

    public async Task ValidateAndProcessBoard()
    {
        try
        {
            Moves moves;
            Flushes flushes;

            do
            {
                moves = FetchNormalized();
                await ProcessNormalized(moves);

                flushes = FetchFlush();
                await ProcessFlushes(flushes);
            } while (moves.Count > 0 || flushes.Count > 0);

            if (_board.IsEmpty()) OnLevelCompleted?.Invoke();
            else if (_board.IsImpossibleToComplete()) OnImpossibleToComplete?.Invoke();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        _appModel.inputDenied = false;
    }

    private async Task ProcessFlushes(Flushes flushes)
    {
        if (flushes.Count == 0) return;
        _board.Flush(flushes);
        _boardViewModel.AnimateFlush(flushes);
        await Task.Delay(_cfg.flushTime.SecondsToMilliseconds());
    }

    private async Task ProcessNormalized(Moves moves)
    {
        if (moves.Count == 0) return;
        _board.MoveBatch(moves);
        _boardViewModel.AnimateMoveBatch(moves);

        await Task.Delay(_cfg.moveTime.SecondsToMilliseconds());
    }

    private Moves FetchNormalized()
    {
        _normalizeWorker.Setup(_board);
        return _normalizeWorker.Work();
    }

    private Flushes FetchFlush()
    {
        _flushWorker.Setup(_board);
        return _flushWorker.Work();
    }


    public void Clear()
    {
        _boardViewModel.Clear();
    }
}