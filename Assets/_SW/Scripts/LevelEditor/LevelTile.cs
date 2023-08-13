using System;
using UnityEngine;
using UnityEngine.Tilemaps;

// ReSharper disable once RequiredBaseTypesConflict
[CreateAssetMenu(fileName = "Add level tile", menuName = "2D/Tiles/Level tile")]
public class LevelTile : Tile
{
    public TileType type;
}

[Serializable]
public enum TileType
{
    Empty = 0,
    Fire = 1,
    Water = 2
}