using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

public class CheckSceneSetup
{
    public static string Execute()
    {
        string result = "";

        // Check InTempoScene
        var scene = EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity", OpenSceneMode.Single);
        result += "=== InTempoScene ===\n";
        foreach (var go in scene.GetRootGameObjects())
        {
            result += $"  [{go.name}]";
            var uiDoc = go.GetComponent<UIDocument>();
            if (uiDoc != null)
            {
                result += $" UIDocument(source={uiDoc.visualTreeAsset?.name}, sortOrder={uiDoc.sortingOrder})";
            }
            var pm = go.GetComponent<PauseManager>();
            if (pm != null) result += " +PauseManager";
            var scm = go.GetComponent<SongCompleteManager>();
            if (scm != null) result += " +SongCompleteManager";
            var ghm = go.GetComponent<GameHudManager>();
            if (ghm != null) result += " +GameHudManager";
            result += "\n";
        }

        // Check LearningScene
        scene = EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/LearningScene.unity", OpenSceneMode.Single);
        result += "\n=== LearningScene ===\n";
        foreach (var go in scene.GetRootGameObjects())
        {
            result += $"  [{go.name}]";
            var uiDoc = go.GetComponent<UIDocument>();
            if (uiDoc != null)
            {
                result += $" UIDocument(source={uiDoc.visualTreeAsset?.name}, sortOrder={uiDoc.sortingOrder})";
            }
            var pm = go.GetComponent<PauseManager>();
            if (pm != null) result += " +PauseManager";
            var scm = go.GetComponent<SongCompleteManager>();
            if (scm != null) result += " +SongCompleteManager";
            var ghm = go.GetComponent<GameHudManager>();
            if (ghm != null) result += " +GameHudManager";
            result += "\n";
        }

        // Return to MainMenu
        EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/MainMenu.unity", OpenSceneMode.Single);

        return result;
    }
}
