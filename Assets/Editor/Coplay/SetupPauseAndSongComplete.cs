using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

public class SetupPauseAndSongComplete
{
    public static string Execute()
    {
        string panelSettingsPath = "Assets/UI Toolkit/PanelSettings.asset";
        string pauseUxmlPath = "Assets/UI Toolkit/PauseMenuVisualTree.uxml";
        string songCompleteUxmlPath = "Assets/UI Toolkit/SongCompleteVisualTree.uxml";

        var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(panelSettingsPath);
        var pauseUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(pauseUxmlPath);
        var songCompleteUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(songCompleteUxmlPath);

        if (panelSettings == null) return "ERROR: PanelSettings not found";
        if (pauseUxml == null) return "ERROR: PauseMenuVisualTree.uxml not found";
        if (songCompleteUxml == null) return "ERROR: SongCompleteVisualTree.uxml not found";

        string result = "";

        // --- InTempoScene ---
        result += SetupScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity",
            panelSettings, pauseUxml, songCompleteUxml);

        // --- LearningScene ---
        result += SetupScene("Assets/LukasMadeStuff/Scenes/LearningScene.unity",
            panelSettings, pauseUxml, songCompleteUxml);

        // Return to MainMenu
        EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/MainMenu.unity", OpenSceneMode.Single);
        result += "Returned to MainMenu.\n";

        return result;
    }

    private static string SetupScene(string scenePath, PanelSettings panelSettings,
        VisualTreeAsset pauseUxml, VisualTreeAsset songCompleteUxml)
    {
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        string result = $"\n=== {scene.name} ===\n";

        // Remove existing PauseMenu if present
        var existingPause = GameObject.Find("PauseMenu");
        if (existingPause != null)
        {
            Object.DestroyImmediate(existingPause);
            result += "  Removed existing PauseMenu.\n";
        }

        // Remove existing SongComplete if present
        var existingSC = GameObject.Find("SongComplete");
        if (existingSC != null)
        {
            Object.DestroyImmediate(existingSC);
            result += "  Removed existing SongComplete.\n";
        }

        // --- Create PauseMenu ---
        var pauseGO = new GameObject("PauseMenu");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(pauseGO, scene);

        var pauseDoc = pauseGO.AddComponent<UIDocument>();
        pauseDoc.panelSettings = panelSettings;
        pauseDoc.visualTreeAsset = pauseUxml;
        pauseDoc.sortingOrder = 100;

        pauseGO.AddComponent<PauseManager>();
        result += "  Created PauseMenu (UIDocument sortOrder=100 + PauseManager)\n";

        // --- Create SongComplete ---
        var scGO = new GameObject("SongComplete");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(scGO, scene);

        var scDoc = scGO.AddComponent<UIDocument>();
        scDoc.panelSettings = panelSettings;
        scDoc.visualTreeAsset = songCompleteUxml;
        scDoc.sortingOrder = 200;

        scGO.AddComponent<SongCompleteManager>();
        result += "  Created SongComplete (UIDocument sortOrder=200 + SongCompleteManager)\n";

        // Save
        EditorSceneManager.SaveScene(scene, scenePath);
        result += "  Scene saved.\n";

        return result;
    }
}
