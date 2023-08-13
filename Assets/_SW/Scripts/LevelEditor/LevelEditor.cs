using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
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
        _tileMap.CompressBounds();

        var level = ScriptableObject.CreateInstance<Level>();
        level.index = _levelIndex;
        level.tiles = GetTilesFromMap(_tileMap).ToList();
        level.width = _tileMap.cellBounds.size.x;

        RepositoryUtils.Save(level);
    }

    private static IEnumerable<SavedTile> GetTilesFromMap(Tilemap map)
    {
        foreach (var pos in map.cellBounds.allPositionsWithin)
        {
            if (map.GetTile(pos) == null) continue;

            var levelTile = map.GetTile<LevelTile>(pos);
            var cellBounds = map.cellBounds;

            yield return new SavedTile
            {
                position = AlignPos(pos, cellBounds),
                tile = levelTile,
            };
        }
    }

    private static Vector3Int AlignPos(Vector3Int pos, BoundsInt bounds)
    {
        pos.x += bounds.size.x / 2;
        pos.y += bounds.size.y / 2;
        return pos;
    }
}