using System;
using UnityEditor;

public class AssetStorage : IRepository
{
    public void Save(Level level)
    {
        AssetDatabase.CreateAsset(level, $"Assets/Resources/levels/{level.assetName}");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public Level Load(int index)
    {
        var level =  AssetDatabase.LoadAssetAtPath<Level>($"Assets/Resources/levels/{Level.GetLevelName(index)}");
        if (level == null) throw new Exception($"Level {index} does not exists!");
        return level;
    }
}