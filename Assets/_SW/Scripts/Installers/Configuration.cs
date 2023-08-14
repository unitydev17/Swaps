using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Configuration", menuName = "Installers/Configuration")]
public class Configuration : ScriptableObjectInstaller<Configuration>
{
    public int forceRunLevel;
    public Level[] levels;
    public GameObject[] itemPrefabs;
    public Vector2 offset;
    public float moveTime;
    public float flushTime;

    public override void InstallBindings()
    {
        Container.BindInstance(itemPrefabs);
        Container.BindInstance(this);
    }
    
    public int GetLevelIndex(int index)
    {
        if (forceRunLevel > 0) index = forceRunLevel;
        return (index - 1) % levels.Length + 1;
    }
}