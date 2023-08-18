using UnityEngine;
using Random = UnityEngine.Random;

public class Item : BaseComponent
{
    private int _index;

    public int index
    {
        get => _index;
        set
        {
            _index = value;
            SetSortingOrder(_index);
        }
    }


    private Animator _animator;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
        SetSortingOrder(_index);
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