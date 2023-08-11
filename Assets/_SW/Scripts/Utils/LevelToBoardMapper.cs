using System;
using System.Collections.Generic;
using UnityEngine;

public static class LevelToBoardMapper
{
    public static Board Map(Level level)
    {
        var items = new List<ItemModel>();

        foreach (var savedTile in level.tiles)
        {
            var type = savedTile.tile.type;

            var model = type switch
            {
                TileType.Empty => (ItemModel) new EmptyItemModel(),
                TileType.Fire => new FireModel(),
                TileType.Water => new WaterItemModel(),
                _ => throw new ArgumentOutOfRangeException()
            };

            model.position = new Vector2Int(savedTile.position.x, savedTile.position.y);
            items.Add(model);
        }

        return new Board {items = items, width = level.width};
    }
}