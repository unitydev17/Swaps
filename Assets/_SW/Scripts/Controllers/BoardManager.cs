using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BoardManager
{
    public static event Action<Flushes> NotifyAnimateFlush;
    public static event Action<Moves> NotifyAnimateMoveBatch;
    public static event Action<int, int, Vector2Int> NotifyAnimateMove;
    public static event Action<int, int, Vector2Int, Vector2Int> NotifyAnimateSwap;

    private Board _board;
    private Transform _root;
    private Configuration _cfg;
    private ItemPool _itemPool;
    private BoardViewModel _boardViewModel;
    private BoardViewModel.Factory _boardViewModelFactory;
    private NormalizeWorker _normalizeWorker;
    private FlushWorker _flushWorker;

    [Inject]
    private void Construct(Configuration cfg, ItemPool itemPool, BoardViewModel.Factory boardViewModelFactory, NormalizeWorker normalizeWorker, FlushWorker flushWorker)
    {
        _cfg = cfg;
        _itemPool = itemPool;
        _boardViewModelFactory = boardViewModelFactory;
        _normalizeWorker = normalizeWorker;
        _flushWorker = flushWorker;
    }

    public void SetBoard(Board board, Transform root)
    {
        _board = board;
        _root = root;
    }

    public void Activate()
    {
        CreateBoard();
    }

    private void CreateBoard()
    {
        var items = CreateItemsFromModel();
        _boardViewModel = _boardViewModelFactory.Create();
        _boardViewModel.Init(items, _board);
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

        item.transform.parent = _root;
        item.index = i;

        var pos = _board.GetPos(i);
        item.transform.localPosition = new Vector3(pos.x * _cfg.offset.x, pos.y * _cfg.offset.y, 0);

        return item;
    }


    public void ProcessMove(Item item, Vector2Int direction)
    {
        if (_boardViewModel.inputDenied) return;

        var currIndex = item.index;
        var nextPos = _board.GetPos(currIndex) + direction;
        var nextIndex = _board.GetIndex(nextPos);

        if (!CheckBounds(nextPos)) return;

        if (CheckEmpty(nextPos))
        {
            if (IsMoveUp(direction)) return;

            _board.Move(currIndex, nextIndex);
            NotifyAnimateMove?.Invoke(currIndex, nextIndex, _board.GetPos(nextIndex));
            return;
        }

        _board.Swap(currIndex, nextIndex);
        NotifyAnimateSwap?.Invoke(currIndex, nextIndex, _board.GetPos(currIndex), _board.GetPos(nextIndex));
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

    public async Task CheckBoard()
    {
        Moves moves;
        Flushes flushes;

        do
        {
            moves = await GetNormalize();
            await ProcessNormalized(moves);

            flushes = await GetFlush();
            await ProcessFlushes(flushes);
            
            
        } while (moves.Count > 0 || flushes.Count > 0);
    }

    private async Task ProcessFlushes(Flushes flushes)
    {
        if (flushes.Count > 0)
        {
            _board.Flush(flushes);
            NotifyAnimateFlush?.Invoke(flushes);

            await Task.Delay(1000);
        }
    }

    private async Task ProcessNormalized(Moves moves)
    {
        if (moves.Count > 0)
        {
            _board.MoveBatch(moves);
            NotifyAnimateMoveBatch?.Invoke(moves);

            await Task.Delay((int) (_cfg.moveTime * 1000));
        }
    }

    private async Task<Moves> GetNormalize()
    {
        _normalizeWorker.Setup(_board);
        return await _normalizeWorker.Work();
    }

    private async Task<Flushes> GetFlush()
    {
        _flushWorker.Setup(_board);
        return await _flushWorker.Work();
    }
}