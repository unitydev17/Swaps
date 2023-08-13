using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BoardManager
{
    private readonly NormalizeWorker _normalizeWorker = new NormalizeWorker();

    private Board _board;
    private Transform _root;
    private Configuration _cfg;
    private ItemPool _itemPool;
    private BoardViewModel _boardViewModel;
    private BoardViewModel.Factory _boardViewModelFactory;

    [Inject]
    private void Construct(Configuration cfg, ItemPool itemPool, BoardViewModel.Factory boardViewModelFactory)
    {
        _cfg = cfg;
        _itemPool = itemPool;
        _boardViewModelFactory = boardViewModelFactory;
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
            return;
        }

        _board.Swap(currIndex, nextIndex);
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
        _normalizeWorker.SetBoard(_board);
        var moves = await _normalizeWorker.Work();
        if (moves.Count > 0) _board.MoveBatch(moves);

        Task.Yield();

        // var flushes = FlushBoard();
    }
}