using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class BoardViewModel : IDisposable
{
    public bool inputDenied;

    private List<Item> _items;
    private Board _board;
    private Configuration _cfg;
    private SignalBus _signalBus;
    private ItemPool _itemPool;

    private void Initialize()
    {
        BoardController.NotifyAnimateMove += OnMove;
        BoardController.NotifyAnimateSwap += OnSwap;
        BoardController.NotifyAnimateMoveBatch += OnNotifyAnimateMoveBatch;
        BoardController.NotifyAnimateFlush += OnFlush;
    }

    public void Dispose()
    {
        BoardController.NotifyAnimateMove -= OnMove;
        BoardController.NotifyAnimateSwap -= OnSwap;
        BoardController.NotifyAnimateMoveBatch -= OnNotifyAnimateMoveBatch;
        BoardController.NotifyAnimateFlush -= OnFlush;
    }


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
        Initialize();
    }

    private void DenyInput()
    {
        inputDenied = true;
    }

    private void AllowInput()
    {
        inputDenied = false;
    }

    private void OnFlush(Flushes flushes)
    {
        foreach (var item in _items.Where(item => flushes.Contains(item.index)))
        {
            item.transform.DOScale(0, 0.5f).OnComplete(() =>
            {
                _items.Remove(item);
                _itemPool.Despawn(item);
            });
        }
    }

    private void OnSwap(int currIndex, int nextIndex, Vector2Int currPos, Vector2Int nextPos)
    {
        var currItem = GetItem(currIndex);
        var nextItem = GetItem(nextIndex);

        DenyInput();
        AnimateMove(currItem, nextPos, () =>
        {
            currItem.index = nextIndex;
            _signalBus.Fire<ValidateBoard>();
            AllowInput();
        });

        AnimateMove(nextItem, currPos, () => { nextItem.index = currIndex; });
    }

    private void OnMove(int index, int nextIndex, Vector2Int newPos)
    {
        var item = GetItem(index);

        DenyInput();
        AnimateMove(item, newPos, () =>
        {
            item.index = nextIndex;
            _signalBus.Fire<ValidateBoard>();
            AllowInput();
        });
    }

    private void OnNotifyAnimateMoveBatch(Moves moves)
    {
        DenyInput();

        var leftMoves = moves.Count;

        moves.ForEach(move =>
        {
            var (currIndex, nextIndex) = move;
            var item = GetItem(currIndex);
            var newPos = _board.GetPos(nextIndex);

            leftMoves--;

            AnimateMove(item, newPos, () =>
            {
                item.index = nextIndex;
                if (leftMoves != 0) return;
                AllowInput();
            });
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
}