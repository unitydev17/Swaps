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
        spriteRenderer.sortingOrder = value;
    }

    public Sprite sprite => spriteRenderer.sprite;

    public virtual void OnDespawned()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnSpawned()
    {
        gameObject.SetActive(true);
    }
}