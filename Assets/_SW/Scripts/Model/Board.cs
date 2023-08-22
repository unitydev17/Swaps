using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public Board()
    {
    }

    public Board(Board board)
    {
        items = new List<ItemModel>(board.items);
        width = board.width;
    }

    public int width;
    public List<ItemModel> items;

    public int height => items.Count / width;

    public ItemModel GetItemModel(int x, int y)
    {
        var index = GetIndex(x, y);
        return items[index];
    }

    public int GetIndex(Vector2Int pos)
    {
        return width * pos.y + pos.x;
    }

    public int GetIndex(int x, int y)
    {
        return width * y + x;
    }

    public Vector2Int GetPos(int index)
    {
        var x = index % width;
        var y = index / width;
        return new Vector2Int(x, y);
    }

    public ItemModel GetItemModel(Vector2Int itemPosition)
    {
        return GetItemModel(itemPosition.x, itemPosition.y);
    }

    public void Move(int currentIndex, int nextIndex)
    {
        SwapModelItems(currentIndex, nextIndex);
    }

    private void SwapModelItems(int currentIndex, int nextIndex)
    {
        var currItemModel = items[currentIndex];
        items[currentIndex] = items[nextIndex];
        items[nextIndex] = currItemModel;
    }

    public void Swap(int currentIndex, int nextIndex)
    {
        SwapModelItems(currentIndex, nextIndex);
    }

    public void MoveBatch(List<(int, int)> moves)
    {
        moves.ForEach(move => Move(move.Item1, move.Item2));
    }

    public void Flush(Flushes flushes)
    {
        foreach (var index in flushes)
        {
            items[index] = new EmptyModel();
        }
    }

    public bool IsEmpty()
    {
        return !items.Any(ItemModel.IsNotEmpty);
    }

    public bool IsImpossibleToComplete()
    {
        var dict = new Dictionary<Type, int>();
        foreach (var type in items.Select(item => item.GetType()))
        {
            if (type == typeof(EmptyModel)) continue;
            dict.TryGetValue(type, out var counter);
            dict[type] = counter + 1;
        }

        return dict.Values.Any(value => value <= 2);
    }
}