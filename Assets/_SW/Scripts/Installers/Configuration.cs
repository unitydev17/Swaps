using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Configuration", menuName = "Installers/Configuration")]
public class Configuration : ScriptableObjectInstaller<Configuration>
{
    public int forceRunLevel;
    public Level[] levels;

    public int MinFlushCount;

    public Sprite sprite;
    public float spriteTransparencyKoeff;
    public float maxSpriteScale;

    public Vector2 offset;
    public float moveTime;
    public float flushTime;

    [Header("Game prefabs")] public BaseComponent[] prefabs;

    [Header("Balloon")] public Vector2 amplitudeRange;
    public Vector2 periodRange;
    public Vector2 speedRange;
    public Vector2 scaleRange;
    public int maxBalloons;
    public Vector2 repeatPeriod;
    public float minHeight;


    public override void InstallBindings()
    {
        Container.BindInstance(prefabs);
        Container.BindInstance(this);
    }

    public int GetLevelIndex(int index)
    {
        if (forceRunLevel > 0) index = forceRunLevel;
        return (index - 1) % levels.Length + 1;
    }
}