using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class BoardViewModel
{
    private List<Item> _items;
    private Board _board;
    private Configuration _cfg;
    private SignalBus _signalBus;
    private ItemPool _itemPool;


    [Inject]
    private void Construct(Configuration cfg, SignalBus signalBus, ItemPool itemPool)
    {
        _cfg = cfg;
        _signalBus = signalBus;
        _itemPool = itemPool;
    }

    public void Init(List<Item> items, Board board)
    {
        _items = items;
        _board = board;
    }

    public void AnimateFlush(Flushes flushes)
    {
        foreach (var item in _items.Where(item => flushes.Contains(item.index)))
        {
            item.Flush();
            DOVirtual.DelayedCall(_cfg.flushTime, () =>
            {
                _items.Remove(item);
                _itemPool.Despawn(item);
            }, false);
        }
    }

    public void AnimateSwap(int currIndex, int nextIndex, Vector2Int currPos, Vector2Int nextPos)
    {
        var currItem = GetItem(currIndex);
        var nextItem = GetItem(nextIndex);

        AnimateMove(currItem, nextPos, () =>
        {
            currItem.index = nextIndex;
            _signalBus.Fire<ValidateBoard>();
        });

        AnimateMove(nextItem, currPos, () => { nextItem.index = currIndex; });
    }

    public void AnimateMove(int index, int nextIndex, Vector2Int newPos)
    {
        var item = GetItem(index);

        AnimateMove(item, newPos, () =>
        {
            item.index = nextIndex;
            _signalBus.Fire<ValidateBoard>();
        });
    }

    public void AnimateMoveBatch(Moves moves)
    {
        moves.ForEach(move =>
        {
            var (currIndex, nextIndex) = move;
            var item = GetItem(currIndex);
            var newPos = _board.GetPos(nextIndex);

            AnimateMove(item, newPos, () => { item.index = nextIndex; });
        });
    }


    private Item GetItem(int index)
    {
        return _items.FirstOrDefault(item => item.index == index);
    }

    private void AnimateMove(Component item, Vector2 position, Action callback)
    {
        var newPos = new Vector3(position.x * _cfg.offset.x, position.y * _cfg.offset.y, 0);
        item.DOKill(true);
        item.transform.DOLocalMove(newPos, _cfg.moveTime);
        DOVirtual.DelayedCall(_cfg.moveTime * 0.75f, () => { callback?.Invoke(); }, ignoreTimeScale: false);
    }


    public class Factory : PlaceholderFactory<BoardViewModel>
    {
    }

    public void Clear()
    {
        foreach (var item in _items)
        {
            _itemPool.Despawn(item);
        }
    }
}