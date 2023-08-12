using System;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public static event Action<int, int, Vector2Int> NotifyMove;
    
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

    // public int GetIndex(ItemModel model)
    // {
    //     return width * model.position.y + model.position.x;
    // } 

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

    // public void Change(ItemModel current, ItemModel neighbour)
    // {
    //     var currIndex = GetIndex(current);
    //     var neighbourIndex = GetIndex(neighbour);
    //     
    //     var currItemModel = items[currIndex];
    //     items[currIndex] = items[neighbourIndex];
    //     items[neighbourIndex] = currItemModel;
    // }

    // public int GetSortedOrder(Vector2Int pos)
    // {
    //     return GetIndex(pos);
    // }

    public void Move(int currentIndex, int nextIndex)
    {
        var currItemModel = items[currentIndex];
        items[currentIndex] = items[nextIndex];
        items[nextIndex] = currItemModel;
        NotifyMove?.Invoke(currentIndex, nextIndex, GetPos(nextIndex));
    }
}