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
    

    [Inject]
    private void Construct(Configuration cfg)
    {
        _cfg = cfg;
    }
    public void Init(List<Item> items, Board board)
    {
        _items = items;
        _board = board;
        Board.NotifyMove += OnMove;
    }

    private void OnMove(int index, int nextIndex, Vector2Int newPos)
    {
        var item = GetItem(index);
        
        AnimateMove(item, newPos, () =>
        {
            item.index = nextIndex;
        });
    }


    private Item GetItem(int index)
    {
        return _items.FirstOrDefault(item => item.index == index);
    }

    private void AnimateMove(Component item, Vector2 position, Action callback)
    {
        inputDenied = true;
        
        var newPos = new Vector3(position.x * _cfg.offset.x, position.y * _cfg.offset.y, 0);
        item.DOKill(true);
        item.transform.DOLocalMove(newPos, _cfg.moveTime).OnComplete(() =>
        {
            callback?.Invoke();
            inputDenied = false;
        });
    }

    public void Dispose()
    {
        Board.NotifyMove -= OnMove;
    }

    public class Factory : PlaceholderFactory<BoardViewModel>
    {
    }

}