using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ConvertHudToUIToolkit
{
    public static string Execute()
    {
        string hudUxmlPath = "Assets/UI Toolkit/GameHudVisualTree.uxml";
        string panelSettingsPath = "Assets/UI Toolkit/PanelSettings.asset";

        var hudUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(hudUxmlPath);
        var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(panelSettingsPath);

        if (hudUxml == null)
            return "ERROR: Could not load GameHudVisualTree.uxml";
        if (panelSettings == null)
            return "ERROR: Could not load PanelSettings.asset";

        string result = "";

        // Process InTempoScene
        result += ProcessScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity", hudUxml, panelSettings, false);

        // Process LearningScene
        result += ProcessScene("Assets/LukasMadeStuff/Scenes/LearningScene.unity", hudUxml, panelSettings, true);

        return result;
    }

    private static string ProcessScene(string scenePath, VisualTreeAsset hudUxml, PanelSettings panelSettings, bool showDelayLabel)
    {
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        string result = $"\n--- {scene.name} ---\n";

        // Find and destroy the old Canvas
        var allObjects = scene.GetRootGameObjects();
        GameObject canvasObj = null;
        foreach (var go in allObjects)
        {
            if (go.GetComponent<Canvas>() != null)
            {
                canvasObj = go;
                break;
            }
        }

        if (canvasObj != null)
        {
            Object.DestroyImmediate(canvasObj);
            result += "  Removed old Canvas\n";
        }
        else
        {
            result += "  No Canvas found (already removed?)\n";
        }

        // Create the new HUD GameObject with UIDocument
        var hudGO = new GameObject("GameHud");
        SceneManager.MoveGameObjectToScene(hudGO, scene);

        var uiDoc = hudGO.AddComponent<UIDocument>();
        uiDoc.panelSettings = panelSettings;
        uiDoc.visualTreeAsset = hudUxml;
        // Set sorting order lower than PauseMenu (100) so pause renders on top
        uiDoc.sortingOrder = 10;

        var hudManager = hudGO.AddComponent<GameHudManager>();
        hudManager.showDelayLabel = showDelayLabel;

        result += $"  Created GameHud (UIDocument + GameHudManager, showDelay={showDelayLabel})\n";

        // Save
        EditorSceneManager.SaveScene(scene);
        result += "  Scene saved.\n";

        return result;
    }
}
