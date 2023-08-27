using System;

public static class TileToBaseComponentTypeMapper
{
    public static BaseComponent.Type Map(TileType type)
    {
        switch (type)
        {
            case TileType.Water:
                return BaseComponent.Type.Water;
            case TileType.Fire:
                return BaseComponent.Type.Fire;
            case TileType.Empty:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        throw new Exception(Constants.Exception_TypeNotMatch);
    }
}