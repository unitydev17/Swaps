using UnityEngine;

public class Balloon : BaseComponent
{
    protected override void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}