using System;
using UnityEditor;

#if !UNITY_EDITOR
using UnityEngine;
#endif

public class LevelStorage : IRepository
{
#if UNITY_EDITOR
    private const string LevelsPath = "Assets/Resources/Levels/";

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
        var path = LevelsPath + Level.GetLevelAsAssetName(index);
        var level = AssetDatabase.LoadAssetAtPath<Level>(path);
        if (level == null) throw new Exception($"Asset Level {index} does not exists! {path}");
        return level;
    }

#else
    private const string LevelsPath = "Levels/";

    public Level Load(int index)
    {
        var path = LevelsPath + Level.GetLevelName(index);
        var level = Resources.Load<Level>(path);
        if (level == null) throw new Exception($"Resource Level {index} does not exists! {path}");
        return level;
    }

    public void Save(Level level)
    {
        throw new NotImplementedException();
    }

#endif
}