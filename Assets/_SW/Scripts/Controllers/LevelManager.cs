using UnityEngine;
using Zenject;

public class LevelManager
{
    private Level _level;

    private Configuration _cfg;
    private Transform _root;

    [Inject]
    private void Construct(Configuration cfg)
    {
        _cfg = cfg;
    }

    public void SetLevel(Level level, Transform root)
    {
        _level = level;
        _root = root;
    }

    public void Activate()
    {
        DrawBoard();
    }

    private void DrawBoard()
    {
        var sortOrder = 0;

        for (var i = 0; i < _level.tiles.Count; i++)
        {
            var savedTile = _level.tiles[i];
            var type = savedTile.tile.type;

            if (type == TileType.Empty) continue;

            var prefab = _cfg.itemPrefabs[(int) type - 1];
            var go = Object.Instantiate(prefab, _root);

            var x = i % _level.width;
            var y = i / _level.width;

            go.transform.localPosition = new Vector3(x * _cfg.offset.x, y * _cfg.offset.y, 0);
            go.GetComponent<SpriteRenderer>().sortingOrder = sortOrder++;
            go.GetComponent<Item>().SetPos(x, y);
        }
    }
}