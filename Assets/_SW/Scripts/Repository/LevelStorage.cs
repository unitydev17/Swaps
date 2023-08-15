using System;
using UnityEditor;
using UnityEngine;

public class LevelStorage : IRepository
{
    private const string LevelsPath = "Levels/";

#if UNITY_EDITOR
    public void Save(Level level)
    {
        if (level == null) return;

        var path = LevelsPath + level.assetName;
        AssetDatabase.CreateAsset(level, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
    }
#else

    public void Save(Level level)
    {
        throw new NotImplementedException();
    }

#endif


    public Level Load(int index)
    {
        var path = LevelsPath + Level.GetLevelName(index);
        var level = Resources.Load<Level>(path);
        if (level == null) throw new Exception($"Level {index} does not exists! {path}");
        return level;
    }
}