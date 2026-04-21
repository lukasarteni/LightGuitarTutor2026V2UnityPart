using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SaveSceneCorrectly
{
    public static string Execute()
    {
        // Get the current active scene
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        
        // Save to the correct path
        bool saved = EditorSceneManager.SaveScene(scene, "Assets/LukasMadeStuff/Scenes/InTempoScene.unity");
        
        // Delete the incorrectly saved scene if it exists
        if (System.IO.File.Exists("Assets/InTempoScene.unity"))
        {
            AssetDatabase.DeleteAsset("Assets/InTempoScene.unity");
        }
        
        return $"Scene saved: {saved}, path: {scene.path}";
    }
}
