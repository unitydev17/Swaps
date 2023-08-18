using UnityEngine;

public class BaseComponent : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSortingOrder(int value)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.sortingOrder = value;
    }

    public Sprite sprite => spriteRenderer.sprite;
}