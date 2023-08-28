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

    private bool isWater => type == Type.Water;

    private void OnEnable()
    {
        _animator.Play(idleAnimation, 0, Random.Range(0, 1f));
    }

    private string idleAnimation => isWater ? Constants.WaterIdleAnimation : Constants.FireIdleAnimation;

    public void Flush()
    {
        _animator.Play(flushAnimation);
    }

    private string flushAnimation => isWater ? Constants.WaterFlushAnimation : Constants.FireFlushAnimation;
}