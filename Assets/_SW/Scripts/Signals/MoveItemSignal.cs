using UnityEngine;

public class MoveItemSignal
{
    public readonly Item item;
    public Vector2Int direction;

    public MoveItemSignal(Item item, Vector2Int direction)
    {
        this.item = item;
        this.direction = direction;
    }
}