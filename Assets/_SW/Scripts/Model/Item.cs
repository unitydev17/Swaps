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
}