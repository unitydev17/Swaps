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

    private Transform _tr;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private static readonly int FlushTrigger = Animator.StringToHash("Flush");

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _tr = transform;
    }

    protected void Deactivate()
    {
        gameObject.SetActive(false);
    }

    protected void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Flush()
    {
        _animator.SetTrigger(FlushTrigger);
    }
}