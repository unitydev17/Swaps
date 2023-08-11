using UnityEngine;
using Zenject;

public class InputController : MonoBehaviour
{
    private bool _tap;
    private Vector3 _prevMousePos;
    private Item _selectedItem;
    private Camera _camera;

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckPress(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            CheckRelease(Input.mousePosition);
        }
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

        var direction = Vector2Int.zero;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            direction.x = delta.x > 0 ? 1 : -1;
        }
        else
        {
            direction.y = delta.y > 0 ? 1 : -1;
        }

        _signalBus.Fire(new MoveItemSignal(_selectedItem, direction));
    }
}