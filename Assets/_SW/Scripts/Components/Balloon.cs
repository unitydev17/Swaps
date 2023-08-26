using UnityEngine;

public class Balloon : BaseComponent
{
    public BalloonModel model;

    protected override void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}