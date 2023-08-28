using UnityEngine;

public class Balloon : BaseComponent
{
    public static int counter = 1;
    
    public BalloonModel model;

    protected override void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        counter++;
    }
}