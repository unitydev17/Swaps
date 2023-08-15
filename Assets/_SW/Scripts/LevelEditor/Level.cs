using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level : ScriptableObject
{
    public int index;
    public int width;
    public List<SavedTile> tiles;

    public string assetName => GetLevelAsAssetName(index);
    public static string GetLevelAsAssetName(int i) => $"{GetLevelName(i)}.asset";
    public static string GetLevelName(int i) => $"Level_{i}";
}

[Serializable]
public class SavedTile
{
    public Vector3Int position;
    public LevelTile tile;
}