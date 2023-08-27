using System.Collections.Generic;
using System.Linq;

public static class LevelToBoardMapper
{
    public static Board Map(Level level)
    {
        return new Board
        {
            items = GetItems(level),
            width = level.width
        };
    }

    private static List<TileType> GetItems(Level level)
    {
        return level.tiles.Select(savedTile => savedTile.tile.type).ToList();
    }
}