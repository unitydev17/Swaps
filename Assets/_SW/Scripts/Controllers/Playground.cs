using EasyButtons;
using UnityEngine;

public class Playground : MonoBehaviour
{
    [SerializeField] private Transform _root;
    [SerializeField] private Vector2Int _dimensions;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private GameObject[] _itemPrefabs;

  

    private int[,] _board;

    private void Start()
    {
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
    }

    private void FillPlayground()
    {
        _board = new int[_dimensions.x, _dimensions.y];
        for (var y = 0; y < _dimensions.y; y++)
        {
            for (var x = 0; x < _dimensions.x; x++)
            {
                _board[x, y] = 0;
            }
        }
    }

    private void FillTestLevel()
    {
        _board = new[,]
        {
            {0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0},
            {0, 0, 2, 1, 0, 0},
            {0, 0, 1, 2, 0, 0},
            {1, 0, 1, 1, 1, 1}
        };
    }

    private void ShowPlayground()
    {
        var height = _board.GetLength(0);
        var width = _board.GetLength(1);
        var sortOrder = 0;
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var yIndex = height - y - 1;
                if (_board[yIndex, x] == 0) continue;
                
                var prefab = _itemPrefabs[_board[yIndex, x] - 1];
                var go = Instantiate(prefab, _root.transform);

                go.transform.localPosition = new Vector3(x * _offset.x, y * _offset.y, 0);
                go.GetComponent<SpriteRenderer>().sortingOrder = sortOrder++;
                go.GetComponent<Item>().SetPos(x, y);
            }
        }
    }

    [Button]
    public void Test()
    {
        FillTestLevel();
        ShowPlayground();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckPress(Input.mousePosition);
        } else
        if (Input.GetMouseButtonUp(0))
        {
            CheckRelease(Input.mousePosition);
        }
    }

    

    private bool _tap;
    private Vector3 _prevMousePos;
    private Item _selectedItem;
    
    private void CheckPress(Vector3 mousePosition)
    {
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector3.zero);
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
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            TryHorizontalMove(delta.x > 0);
        }
    }

    private void TryHorizontalMove(bool right)
    {
        if (right)
        {
            // if (_selectedItem.type == Item.Type.Fire && _board[_selectedItem.x + 1, _board.GetLength(1) - selectedItem.y)] != 2) // todo: types/classes
            // {
            //     _selectedItem.transform.DOLocalMoveX((_selectedItem.x + 1) * _offset.x, 1);
            //     _board[_selectedItem.x, _selectedItem.y] = 0;
            //     _board[_selectedItem.x + 1, _selectedItem.y] = 2;
            // } 
        }
        else
        {
            
        }
        
    }
    
    
}