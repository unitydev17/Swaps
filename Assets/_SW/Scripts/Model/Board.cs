using System;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public static event Action<List<(int, int)>> NotifyMoveBatch;
    public static event Action<int, int, Vector2Int> NotifyMove;
    public static event Action<int, int, Vector2Int, Vector2Int> NotifySwap;

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

    private ItemModel GetItemModel(int x, int y)
    {
        return items[width * y + x];
    }

    public int GetIndex(Vector2Int pos)
    {
        return width * pos.y + pos.x;
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

    public ItemModel GetItemModel(int index)
    {
        return items[index];
    }

    public void Move(int currentIndex, int nextIndex)
    {
        MoveSilent(currentIndex, nextIndex);
        NotifyMove?.Invoke(currentIndex, nextIndex, GetPos(nextIndex));
    }

    public void MoveSilent(int currentIndex, int nextIndex)
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
        NotifySwap?.Invoke(currentIndex, nextIndex, GetPos(currentIndex), GetPos(nextIndex));
    }
    
    public void MoveBatch(List<(int, int)> moves)
    {
        moves.ForEach(move => MoveSilent(move.Item1, move.Item2));
        NotifyMoveBatch?.Invoke(moves);
    }
}