using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BoardController : IInitializable, IDisposable
{
    public static event Action OnLevelCompleted;
    public static event Action OnImpossibleToComplete;


    private BoardViewModel _boardViewModel;
    private Board _board;
    private Transform _pivot;

    private Configuration _cfg;
    private BoardViewModel.Factory _boardViewModelFactory;
    private NormalizeWorker _normalizeWorker;
    private IFlushWorker _flushWorker;
    private AppModel _appModel;
    private Camera _camera;

    private GamePool _commonPool;


    [Inject]
    private void Construct(
        AppModel appModel,
        Configuration cfg,
        BoardViewModel.Factory boardViewModelFactory,
        NormalizeWorker normalizeWorker,
        IFlushWorker flushWorker,
        GamePool commonPool)
    {
        _appModel = appModel;
        _cfg = cfg;
        _boardViewModelFactory = boardViewModelFactory;
        _normalizeWorker = normalizeWorker;
        _flushWorker = flushWorker;
        _commonPool = commonPool;
    }


    public void Initialize()
    {
        InputController.OnMove += ProcessMove;
    }

    public void Dispose()
    {
        InputController.OnMove -= ProcessMove;
    }


    public void SetBoard(Board board, Transform pivot, Camera camera)
    {
        _board = board;
        _pivot = pivot;
        _camera = camera;
    }

    public void Activate()
    {
        CreateBoard();
        ValidateAndProcessBoard();
    }

    private void CreateBoard()
    {
        ResetRootScale();
        AlignPivot();

        var items = CreateItemsFromTiles().ToList();
        _boardViewModel = _boardViewModelFactory.Create();
        _boardViewModel.Init(items, _board);

        CorrectRootScale();
    }

    private void ResetRootScale()
    {
        _pivot.parent.localScale = Vector3.one;
    }

    private void AlignPivot()
    {
        var pivotOffsetX = (_board.width - 1) * _cfg.offset.x * 0.5f;
        _pivot.localPosition = Vector3.left * pivotOffsetX;
    }

    private void CorrectRootScale()
    {
        var spriteInUnits = _cfg.sprite.texture.width / _cfg.sprite.pixelsPerUnit;
        var scale = _camera.aspect * _camera.orthographicSize * 2 / (_board.width * spriteInUnits * _cfg.spriteTransparencyKoeff);
        scale = Mathf.Min(scale, _cfg.maxSpriteScale);
        _pivot.parent.localScale = Vector3.one * scale;
    }

    private IEnumerable<Item> CreateItemsFromTiles()
    {
        for (var i = 0; i < _board.items.Count; i++)
        {
            var tileType = _board.items[i];
            if (tileType == TileType.Empty) continue;

            var item = CreateItem(tileType, i);
            yield return item;
        }
    }

    private Item CreateItem(TileType type, int i)
    {
        var item = (Item) _commonPool.Spawn(TileToBaseComponentTypeMapper.Map(type));
        item.transform.parent = _pivot;
        item.index = i;

        var pos = _board.GetPos(i);
        item.transform.localPosition = new Vector3(pos.x * _cfg.offset.x, pos.y * _cfg.offset.y, 0);

        return item;
    }


    private void ProcessMove(Item item, Vector2Int direction)
    {
        if (_appModel.inputDenied) return;
        _appModel.inputDenied = true;

        var currIndex = item.index;
        var nextPos = _board.GetPos(currIndex) + direction;
        var nextIndex = _board.GetIndex(nextPos);

        if (!CheckBounds(nextPos))
        {
            _appModel.inputDenied = false;
            return;
        }

        if (CheckEmpty(nextPos))
        {
            if (IsMoveUp(direction))
            {
                _appModel.inputDenied = false;
                return;
            }

            _board.Move(currIndex, nextIndex);
            _boardViewModel.AnimateMove(currIndex, nextIndex, _board.GetPos(nextIndex), ValidateAndProcessBoard);
            return;
        }

        _board.Swap(currIndex, nextIndex);
        _boardViewModel.AnimateSwap(currIndex, nextIndex, _board.GetPos(currIndex), _board.GetPos(nextIndex), ValidateAndProcessBoard);
    }

    private static bool IsMoveUp(Vector2Int direction)
    {
        return direction.y > 0;
    }

    private bool CheckEmpty(Vector2Int nextPos)
    {
        return _board.GetType(nextPos) == TileType.Empty;
    }

    private bool CheckBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < _board.width && pos.y >= 0 && pos.y < _board.height;
    }

    private void ValidateAndProcessBoard()
    {
        _appModel.inputDenied = true;
        Flushes flushes;

        var moves = FetchNormalized();
        ProcessNormalized(moves, () =>
        {
            flushes = FetchFlush();
            ProcessFlushes(flushes, () =>
            {
                if (moves.Count > 0 || flushes.Count > 0)
                {
                    ValidateAndProcessBoard();
                }
                else
                {
                    FinishActions();
                }
            });
        });

        void FinishActions()
        {
            if (_board.IsEmpty()) OnLevelCompleted?.Invoke();
            else if (_board.IsImpossibleToComplete()) OnImpossibleToComplete?.Invoke();
            _appModel.inputDenied = false;
        }
    }

    private void ProcessFlushes(Flushes flushes, Action callback)
    {
        if (flushes.Count == 0)
        {
            callback?.Invoke();
            return;
        }

        _board.Flush(flushes);
        _boardViewModel.AnimateFlush(flushes, callback);
    }

    private void ProcessNormalized(Moves moves, Action callback)
    {
        if (moves.Count == 0)
        {
            callback?.Invoke();
            return;
        }

        _board.MoveBatch(moves);
        _boardViewModel.AnimateMoveBatch(moves, callback);
    }

    private Moves FetchNormalized()
    {
        _normalizeWorker.Setup(_board);
        return _normalizeWorker.Work();
    }

    private Flushes FetchFlush()
    {
        _flushWorker.Setup(_board);
        return _flushWorker.Work();
    }


    public void Clear()
    {
        _boardViewModel.Clear();
    }
}