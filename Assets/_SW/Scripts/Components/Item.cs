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
    private Animator _animator;
    private static readonly int FlushTrigger = Animator.StringToHash("Flush");

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
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
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;
    }

    public void Flush()
    {
        _animator.SetTrigger(FlushTrigger);
    }
}