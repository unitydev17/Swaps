using System;

public static class TileToBaseComponentTypeMapper
{
    public static BaseComponent.Type Map(TileType type)
    {
        var result = type switch
        {
            TileType.Water => BaseComponent.Type.Water,
            TileType.Fire => BaseComponent.Type.Fire,
            TileType.Empty => ThrowException(type),
            _ => ThrowException(type)
        };
        return result;
    }

    private static BaseComponent.Type ThrowException(TileType type)
    {
        throw new Exception(string.Format(Constants.ExceptionTypeNotMatch, type));
    }
}