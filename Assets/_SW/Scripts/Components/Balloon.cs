using UnityEngine;

public abstract class Balloon : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetSortingOrder(int value)
    {
        _spriteRenderer.sortingOrder = value;
    }
}