using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

public class WireNewPauseManagers
{
    public static string Execute()
    {
        string panelSettingsPath = "Assets/UI Toolkit/PanelSettings.asset";
        string pauseUxmlPath = "Assets/UI Toolkit/PauseMenuVisualTree.uxml";

        var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(panelSettingsPath);
        var pauseUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(pauseUxmlPath);

        if (panelSettings == null) return "ERROR: PanelSettings not found";
        if (pauseUxml == null) return "ERROR: PauseMenuVisualTree.uxml not found";

        string result = "";

        // --- InTempoScene ---
        var scene = EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity", OpenSceneMode.Single);
        result += "=== InTempoScene ===\n";

        // Remove existing PauseMenu
        var existing = GameObject.Find("PauseMenu");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
            result += "  Removed old PauseMenu.\n";
        }

        // Create new PauseMenu with InTempoPauseManager
        var pauseGO = new GameObject("PauseMenu");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(pauseGO, scene);

        var pauseDoc = pauseGO.AddComponent<UIDocument>();
        pauseDoc.panelSettings = panelSettings;
        pauseDoc.visualTreeAsset = pauseUxml;
        pauseDoc.sortingOrder = 100;

        pauseGO.AddComponent<InTempoPauseManager>();
        result += "  Created PauseMenu with InTempoPauseManager (sortOrder=100)\n";

        EditorSceneManager.SaveScene(scene, "Assets/LukasMadeStuff/Scenes/InTempoScene.unity");
        result += "  Saved.\n";

        // --- LearningScene ---
        scene = EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/LearningScene.unity", OpenSceneMode.Single);
        result += "\n=== LearningScene ===\n";

        existing = GameObject.Find("PauseMenu");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
            result += "  Removed old PauseMenu.\n";
        }

        pauseGO = new GameObject("PauseMenu");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(pauseGO, scene);

        pauseDoc = pauseGO.AddComponent<UIDocument>();
        pauseDoc.panelSettings = panelSettings;
        pauseDoc.visualTreeAsset = pauseUxml;
        pauseDoc.sortingOrder = 100;

        pauseGO.AddComponent<LearningPauseManager>();
        result += "  Created PauseMenu with LearningPauseManager (sortOrder=100)\n";

        EditorSceneManager.SaveScene(scene, "Assets/LukasMadeStuff/Scenes/LearningScene.unity");
        result += "  Saved.\n";

        // Return to InTempoScene (user had it open)
        EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity", OpenSceneMode.Single);
        result += "\nReturned to InTempoScene.\n";

        return result;
    }
}
