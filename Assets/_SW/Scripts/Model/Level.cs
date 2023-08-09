using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level : ScriptableObject
{
    public int index;
    public Vector3 dimension = new Vector3(6,6);
    public List<SavedTile> tiles;
    
    public int[,] data;

    public string assetName => GetLevelName(index);
    public static string GetLevelName(int i) => $"Level_{i}.asset";
}

[Serializable]
public class SavedTile
{
    public Vector3Int position;
    public LevelTile tile;
}