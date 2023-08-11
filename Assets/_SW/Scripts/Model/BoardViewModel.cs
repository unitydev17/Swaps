using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class BoardViewModel
{
    private List<Item> _items;
    private Board _board;
    private Configuration _cfg;

    public void Init(List<Item> items, Board board)
    {
        _items = items;
        _board = board;
    }

    [Inject]
    private void Construct(Configuration cfg)
    {
        _cfg = cfg;
    }

    public void AnimateChange(Item item, Vector2Int direction)
    {
        var deltaMove = new Vector3(direction.x * _cfg.offset.x, direction.y * _cfg.offset.y, 0);
        item.transform.DOLocalMove(item.transform.localPosition + deltaMove, 1);
    }

    public class Factory : PlaceholderFactory<BoardViewModel>
    {
    }
}