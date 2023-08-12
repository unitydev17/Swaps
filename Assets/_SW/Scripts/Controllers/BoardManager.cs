using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BoardManager
{
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
        var moves = await NormalizeBoard();
        
        if (moves.Count > 0) _board.MoveBatch(moves);

        // var flushes = FlushBoard();
    }

    // private Task<List<int>> FlushBoard()
    // {
    //     
    //     
    //     for (var i = 0; i < _board.items.Count; i++)
    //     {
    //         
    //         // _board.items[i]
    //         
    //     }
    // }

    private async Task<List<(int, int)>> NormalizeBoard()
    {
        var moves = new List<(int, int)>();

        var normBoard = new Board(_board);
        bool hadMoves;

        do
        {
            hadMoves = false;

            for (var i = normBoard.width; i < normBoard.items.Count; i++)
            {
                var currPos = normBoard.GetPos(i);
                if (normBoard.GetItemModel(currPos) is EmptyModel) continue;

                var nextPos = currPos;
                var targetPos = currPos;
                var hasEmpties = false;

                for (var y = currPos.y - 1; y >= 0; y--)
                {
                    nextPos.y = y;
                    var nextItem = normBoard.GetItemModel(nextPos);

                    if (nextItem is EmptyModel)
                    {
                        hasEmpties = true;
                        targetPos = nextPos;
                    }
                    else break;
                }

                if (!hasEmpties) continue;

                var targetIndex = normBoard.GetIndex(targetPos);
                normBoard.MoveSilent(i, targetIndex);

                hadMoves = true;
                moves.Add((i, targetIndex));

                await Task.Yield();
            }
        } while (hadMoves);

        return moves;
    }
}