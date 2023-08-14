using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BoardController
{
    private BoardViewModel _boardViewModel;
    private Board _board;
    private Transform _pivot;

    private SignalBus _signalBus;
    private Configuration _cfg;
    private ItemPool _itemPool;
    private BoardViewModel.Factory _boardViewModelFactory;
    private NormalizeWorker _normalizeWorker;
    private FlushWorker _flushWorker;
    private AppModel _appModel;


    [Inject]
    private void Construct(
        AppModel appModel,
        Configuration cfg,
        SignalBus signalBus,
        ItemPool itemPool,
        BoardViewModel.Factory boardViewModelFactory,
        NormalizeWorker normalizeWorker,
        FlushWorker flushWorker)
    {
        _appModel = appModel;
        _cfg = cfg;
        _signalBus = signalBus;
        _itemPool = itemPool;
        _boardViewModelFactory = boardViewModelFactory;
        _normalizeWorker = normalizeWorker;
        _flushWorker = flushWorker;
    }

    public void SetBoard(Board board, Transform pivot)
    {
        _board = board;
        _pivot = pivot;
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
        var scale = Vector3.one * _cfg.scaleCurve.Evaluate(_board.width / 10f);
        scale *= Screen.width / (float) Screen.height * _cfg.GetTargetRatio();
        _pivot.parent.localScale = scale;
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
        var item = _itemPool.Spawn(itemModel);

        item.transform.parent = _pivot;
        item.index = i;

        var pos = _board.GetPos(i);
        item.transform.localPosition = new Vector3(pos.x * _cfg.offset.x, pos.y * _cfg.offset.y, 0);

        return item;
    }


    public void ProcessMove(Item item, Vector2Int direction)
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
            _boardViewModel.AnimateMove(currIndex, nextIndex, _board.GetPos(nextIndex));
            return;
        }

        _board.Swap(currIndex, nextIndex);
        _boardViewModel.AnimateSwap(currIndex, nextIndex, _board.GetPos(currIndex), _board.GetPos(nextIndex));
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
        Moves moves;
        Flushes flushes;

        do
        {
            moves = await FetchNormalized();
            await ProcessNormalized(moves);

            flushes = await FetchFlush();
            await ProcessFlushes(flushes);
        } while (moves.Count > 0 || flushes.Count > 0);

        if (_board.IsEmpty()) _signalBus.Fire<LevelCompletedSignal>();

        _appModel.inputDenied = false;
    }

    private async Task ProcessFlushes(Flushes flushes)
    {
        if (flushes.Count == 0) return;

        _board.Flush(flushes);
        _boardViewModel.AnimateFlush(flushes);

        await Task.Delay((int) (_cfg.flushTime * 1000));
    }

    private async Task ProcessNormalized(Moves moves)
    {
        if (moves.Count == 0) return;

        _board.MoveBatch(moves);
        _boardViewModel.AnimateMoveBatch(moves);

        await Task.Delay((int) (_cfg.moveTime * 1000));
    }

    private async Task<Moves> FetchNormalized()
    {
        _normalizeWorker.Setup(_board);
        return await _normalizeWorker.Work();
    }

    private async Task<Flushes> FetchFlush()
    {
        _flushWorker.Setup(_board);
        return await _flushWorker.Work();
    }


    public void Clear()
    {
        _boardViewModel.Clear();
    }
}