using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private int _levelIndex;


    [Button]
    private void LoadLevel()
    {
        var level = RepositoryUtils.LoadLevel(_levelIndex);
        if (level == null) return;

        ClearLevel();

        foreach (var savedTile in level.tiles)
        {
            _tileMap.SetTile(savedTile.position, savedTile.tile);
        }
    }

    [Button]
    private void ClearLevel()
    {
        _tileMap.ClearAllTiles();
    }

    [Button]
    private void SaveLevel()
    {
        var level = ScriptableObject.CreateInstance<Level>();
        level.index = _levelIndex;
        level.tiles = GetTilesFromMap(_tileMap).ToList();

        RepositoryUtils.Save(level);
    }

    private static IEnumerable<SavedTile> GetTilesFromMap(Tilemap map)
    {
        foreach (var pos in map.cellBounds.allPositionsWithin)
        {
            if (map.GetTile(pos))
            {
                var levelTile = map.GetTile<LevelTile>(pos);
                yield return new SavedTile
                {
                    position = pos,
                    tile = levelTile
                };
            }
        }
    }

    private int[,] ReadTiles()
    {
        var tiles = new List<int>();

        var bounds = _tileMap.cellBounds;
        var (from, to) = (0, 0);

        for (var y = bounds.max.y; y > bounds.min.y; y--)
        {
            for (var x = bounds.min.x; x < bounds.max.x; x++)
            {
                var tile = _tileMap.GetTile<LevelTile>(new Vector3Int(x, y, 0));
                if (tile == null)
                {
                    if (from != 0 && to == 0) to = x;
                    continue;
                }

                if (from == 0) from = x;

                tiles.Add((int) tile.type);
            }
        }

        var width = to - from;
        var height = tiles.Count / width;

        return TilesListToArray();

        int[,] TilesListToArray()
        {
            var result = new int[height, width];

            for (var i = 0; i < tiles.Count; i++)
            {
                var y = i / width;
                var x = i % width;
                result[y, x] = tiles[i];
            }

            return result;
        }
    }
}