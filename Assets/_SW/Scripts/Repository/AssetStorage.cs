using System;
using UnityEditor;

public class AssetStorage : IRepository
{
    private string _levelsPath = "Assets/Resources/levels/";

    public void Save(Level level)
    {
        var path = _levelsPath + level.assetName;
        AssetDatabase.CreateAsset(level, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public Level Load(int index)
    {
        var path = _levelsPath + Level.GetLevelName(index);
        var level = AssetDatabase.LoadAssetAtPath<Level>(path);
        if (level == null) throw new Exception($"Level {index} does not exists!");
        return level;
    }
}