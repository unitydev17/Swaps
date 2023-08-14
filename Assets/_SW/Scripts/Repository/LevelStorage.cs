using System;
using UnityEditor;

public class LevelStorage : IRepository
{
    private const string LevelsPath = "Assets/Resources/levels/";

    public void Save(Level level)
    {
        if (level == null) return;

        var path = LevelsPath + level.assetName;
        AssetDatabase.CreateAsset(level, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public Level Load(int index)
    {
        var path = LevelsPath + Level.GetLevelName(index);
        var level = AssetDatabase.LoadAssetAtPath<Level>(path);
        if (level == null) throw new Exception($"Level {index} does not exists!");
        return level;
    }
}