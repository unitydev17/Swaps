using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Configuration", menuName = "Installers/Configuration")]
public class Configuration : ScriptableObjectInstaller<Configuration>
{
    public GameObject[] itemPrefabs;
    public Vector2 offset;

    public override void InstallBindings()
    {
        Container.BindInstance(itemPrefabs);
        Container.BindInstance(this);
    }
}