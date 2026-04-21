using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Coplay.Controllers.Functions;

public class SetupPauseMenu2
{
    public static string Execute()
    {
        string result = "";

        // --- INTEMPO SCENE ---
        result += CoplayTools.OpenScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity") + "\n";

        // Check if PauseMenu already exists, remove it if so
        var existing = GameObject.Find("PauseMenu");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
            result += "Removed existing PauseMenu from InTempoScene.\n";
        }

        result += CoplayTools.CreateGameObject("PauseMenu", "0,0,0") + "\n";
        result += CoplayTools.AddComponent("PauseMenu", "UnityEngine.UIElements.UIDocument") + "\n";
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "panelSettings",
            "Assets/UI Toolkit/PanelSettings.asset") + "\n";
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "sourceAsset",
            "Assets/UI Toolkit/PauseMenuVisualTree.uxml") + "\n";
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "sortingOrder", "100") + "\n";
        result += CoplayTools.AddComponent("PauseMenu", "PauseManager") + "\n";

        // Save to original path
        EditorSceneManager.SaveScene(
            EditorSceneManager.GetActiveScene(),
            "Assets/LukasMadeStuff/Scenes/InTempoScene.unity");
        result += "Saved InTempoScene to correct path.\n";

        // --- LEARNING SCENE ---
        result += CoplayTools.OpenScene("Assets/LukasMadeStuff/Scenes/LearningScene.unity") + "\n";

        existing = GameObject.Find("PauseMenu");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
            result += "Removed existing PauseMenu from LearningScene.\n";
        }

        result += CoplayTools.CreateGameObject("PauseMenu", "0,0,0") + "\n";
        result += CoplayTools.AddComponent("PauseMenu", "UnityEngine.UIElements.UIDocument") + "\n";
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "panelSettings",
            "Assets/UI Toolkit/PanelSettings.asset") + "\n";
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "sourceAsset",
            "Assets/UI Toolkit/PauseMenuVisualTree.uxml") + "\n";
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "sortingOrder", "100") + "\n";
        result += CoplayTools.AddComponent("PauseMenu", "PauseManager") + "\n";

        EditorSceneManager.SaveScene(
            EditorSceneManager.GetActiveScene(),
            "Assets/LukasMadeStuff/Scenes/LearningScene.unity");
        result += "Saved LearningScene to correct path.\n";

        // Return to InTempo scene
        result += CoplayTools.OpenScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity") + "\n";

        return result;
    }
}
