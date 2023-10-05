using System;
using UniRx;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static event Action<Item, Vector2Int> OnMove;

    [SerializeField] private float _threshold;

    private bool _tap;
    private Vector3 _prevMousePos;
    private Item _selectedItem;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;

        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(x => CheckPress(Input.mousePosition));

        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Subscribe(x => CheckRelease(Input.mousePosition));
    }

    private void CheckPress(Vector3 mousePosition)
    {
        var hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(mousePosition), Vector3.zero);
        if (!hit.collider) return;

        _tap = true;
        _prevMousePos = mousePosition;
        _selectedItem = hit.collider.GetComponent<Item>();
    }

    private void CheckRelease(Vector3 mousePosition)
    {
        if (!_tap) return;
        _tap = false;

        var delta = mousePosition - _prevMousePos;
        if (delta.sqrMagnitude < _threshold) return;

        var direction = Vector2Int.zero;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            direction.x = delta.x > 0 ? 1 : -1;
        }
        else
        {
            direction.y = delta.y > 0 ? 1 : -1;
        }

        OnMove?.Invoke(_selectedItem, direction);
    }
}