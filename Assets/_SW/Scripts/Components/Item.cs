using UnityEngine;

public class Item : MonoBehaviour
{
    private int _index;

    public int index
    {
        get => _index;
        set
        {
            _index = value;
            _spriteRenderer.sortingOrder = _index;
        }
    }

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSortingOrder(int value)
    {
        _spriteRenderer.sortingOrder = value;
    }

    public int sortingOrder => _spriteRenderer.sortingOrder;

    protected void Deactivate()
    {
        gameObject.SetActive(false);
    }

    protected void Activate()
    {
        transform.localScale = Vector3.one;
    }
}