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
    [SerializeField] private LevelTile _emptyTile;


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
        LoadLevel();
    }

    private IEnumerable<SavedTile> GetTilesFromMap(Tilemap map)
    {
        foreach (var pos in map.cellBounds.allPositionsWithin)
        {
            var levelTile = map.GetTile(pos) == null ? _emptyTile : map.GetTile<LevelTile>(pos);

            yield return new SavedTile
            {
                position = pos,
                tile = levelTile,
            };
        }
    }
}