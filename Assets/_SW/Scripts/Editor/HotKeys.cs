using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class HotKeys : MonoBehaviour
{
    [MenuItem("Game/Editor #1")]
    private static void Editor()
    {
        EditorSceneManager.OpenScene("Assets/_SW/Scenes/Editor.unity");
    }

    [MenuItem("Game/Main #2")]
    private static void Main()
    {
        EditorSceneManager.OpenScene("Assets/_SW/Scenes/Main.unity");
    }
}