using UnityEngine;
using Random = UnityEngine.Random;

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

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private bool isWater => GetType() == typeof(WaterItem);

    private void OnEnable()
    {
        _animator.Play(isWater ? "WaterIdle" : "FireIdle", 0, Random.Range(0, 1f));
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
        _animator.Play(isWater ? "WaterFlush" : "FireFlush");
    }
}