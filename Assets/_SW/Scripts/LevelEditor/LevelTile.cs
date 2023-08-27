using UnityEngine;
using UnityEngine.Tilemaps;

// ReSharper disable once RequiredBaseTypesConflict
[CreateAssetMenu(fileName = "LevelTile", menuName = "2D/Tiles/Level tile")]
public class LevelTile : Tile
{
    public TileType type;
}