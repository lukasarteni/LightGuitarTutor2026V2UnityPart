using UnityEditor;
using UnityEditor.SceneManagement;

public class SaveInTempoScene
{
    public static string Execute()
    {
        EditorSceneManager.SaveOpenScenes();
        return "Saved all open scenes.";
    }
}
