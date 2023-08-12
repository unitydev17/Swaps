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

    private void DenyInput()
    {
        inputDenied = true;
    }

    private void AllowInput()
    {
        inputDenied = false;
    }

    [Inject]
    private void Construct(Configuration cfg, SignalBus signalBus)
    {
        _cfg = cfg;
        _signalBus = signalBus;
    }

    public void Init(List<Item> items, Board board)
    {
        _items = items;
        _board = board;
        Board.NotifyMove += OnMove;
        Board.NotifySwap += OnSwap;
        Board.NotifyMoveBatch += OnMoveBatch;
    }

    private void OnSwap(int currIndex, int nextIndex, Vector2Int currPos, Vector2Int nextPos)
    {
        var currItem = GetItem(currIndex);
        var nextItem = GetItem(nextIndex);

        DenyInput();
        AnimateMove(currItem, nextPos, () =>
        {
            currItem.index = nextIndex;
            _signalBus.Fire<CheckBoardSignal>();
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
            _signalBus.Fire<CheckBoardSignal>();
            AllowInput();
        });
    }

    private void OnMoveBatch(List<(int, int)> moves)
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
                _signalBus.Fire<CheckBoardSignal>();
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

    public void Dispose()
    {
        Board.NotifyMove -= OnMove;
        Board.NotifySwap -= OnSwap;
        Board.NotifyMoveBatch -= OnMoveBatch;
    }

    public class Factory : PlaceholderFactory<BoardViewModel>
    {
    }
}