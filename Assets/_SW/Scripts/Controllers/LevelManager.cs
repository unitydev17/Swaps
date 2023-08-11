using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager
{
    private BoardViewModel _boardViewModel;
    private Board _board;
    private Transform _root;
    private Configuration _cfg;
    private ItemPool _itemPool;
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
        var items = new List<Item>();
        var sortOrder = 0;

        for (var i = 0; i < _board.items.Count; i++)
        {
            var itemModel = _board.items[i];
            if (itemModel is EmptyItemModel) continue;

            var item = CreateItem(itemModel, i);
            items.Add(item);
        }

        _boardViewModel = _boardViewModelFactory.Create();
        _boardViewModel.Init(items, _board);


        Item CreateItem(ItemModel itemModel, int i)
        {
            var item = _itemPool.Spawn(itemModel);

            var transform = item.transform;
            transform.parent = _root;

            var pos = _board.GetPos(i);
            item.SetPos(pos.x, pos.y);
            item.index = i;

            transform.localPosition = new Vector3(pos.x * _cfg.offset.x, pos.y * _cfg.offset.y, 0);
            item.GetComponent<SpriteRenderer>().sortingOrder = sortOrder++;
            return item;
        }
    }


    public void ProcessMove(Item item, Vector2Int direction)
    {
        var currIndex = item.index;
        var nextPos = _board.GetPos(currIndex) + direction;

        if (!CheckBounds(nextPos)) return;

        if (CheckEmpty(nextPos))
        {
            var nextIndex = _board.GetIndex(nextPos);
            _board.Change(currIndex, nextIndex);

            item.index = nextIndex;
            _boardViewModel.AnimateChange(item, direction);
            return;
        }


        // var nextPos = item.position + direction;
        //
        // if (!CheckBounds(nextPos)) return;
        //
        // if (CheckEmpty(nextPos))
        // {
        //     var deltaMove = new Vector3(direction.x * _cfg.offset.x, direction.y * _cfg.offset.y, 0);
        //     item.transform.DOLocalMove(item.transform.localPosition + deltaMove, 1);
        //     return;
        // }
        // if (!CheckSameNeighbour(item, nextPos))
        // {
        //     var current = _board.GetItemModel(item.position);
        //     var neighbour = _board.GetItemModel(nextPos.x, nextPos.y);
        //     _board.Change(current, neighbour);
        // }
    }

    private bool CheckEmpty(Vector2Int nextPos)
    {
        return _board.GetItemModel(nextPos) is EmptyItemModel;
    }

    private bool CheckSameNeighbour(Item item, Vector2Int nextPos)
    {
        var neighbour = _board.GetItemModel(nextPos);
        var current = _board.GetItemModel(item.position);
        return current.GetType() == neighbour.GetType();
    }

    private bool CheckBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < _board.width && pos.y >=0 && pos.y < _board.height;
    }
}