using System;
using System.Collections.Generic;

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
                TileType.Empty => (ItemModel) new EmptyModel(),
                TileType.Fire => new FireModel(),
                TileType.Water => new WaterModel(),
                _ => throw new ArgumentOutOfRangeException()
            };

            items.Add(model);
        }

        return new Board
        {
            items = items, 
            width = level.width
        };
    }
}