using UnityEngine;
using Zenject;

public class BaseComponent : MonoBehaviour, IPoolable
{
    public enum Type
    {
        Water,
        Fire,
        BlueBalloon,
        OrangeBalloon
    }

    public Type type;

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

    public virtual void OnDespawned()
    {
    }

    public virtual void OnSpawned()
    {
    }
}