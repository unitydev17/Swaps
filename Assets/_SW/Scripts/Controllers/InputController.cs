using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private bool _tap;
    private Vector3 _prevMousePos;
    private Item _selectedItem;
    private Camera _camera;

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
        Debug.Log(_selectedItem);
    }

    private void CheckRelease(Vector3 mousePosition)
    {
        if (!_tap) return;
        _tap = false;

        var delta = mousePosition - _prevMousePos;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            TryHorizontalMove(delta.x > 0);
        }
    }

    private void TryHorizontalMove(bool right)
    {
       
    }
}