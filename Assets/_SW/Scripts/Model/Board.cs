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
        items = new List<TileType>(board.items);
        width = board.width;
    }

    public int width;
    public List<TileType> items;

    public int height => items.Count / width;

    public TileType GetTileType(int x, int y)
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

    public TileType GetType(Vector2Int itemPosition)
    {
        return GetTileType(itemPosition.x, itemPosition.y);
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
            items[index] = TileType.Empty;
        }
    }

    public bool IsEmpty()
    {
        return items.All(item => item is TileType.Empty);
    }

    public bool IsImpossibleToComplete()
    {
        var dict = new Dictionary<TileType, int>();

        items.ForEach(item =>
        {
            if (item != TileType.Empty)
            {
                dict.TryGetValue(item, out var counter);
                dict[item] = counter + 1;
            }
        });

        return dict.Values.Any(value => value <= 2);
    }
}