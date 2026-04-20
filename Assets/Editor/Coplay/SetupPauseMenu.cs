using UnityEngine;
using Coplay.Controllers.Functions;

public class SetupPauseMenu
{
    public static string Execute()
    {
        string result = "";

        // --- INTEMPO SCENE ---
        result += CoplayTools.OpenScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity") + "\n";

        // Create PauseMenu GameObject
        result += CoplayTools.CreateGameObject("PauseMenu", "0,0,0") + "\n";

        // Add UIDocument component
        result += CoplayTools.AddComponent("PauseMenu", "UnityEngine.UIElements.UIDocument") + "\n";

        // Assign the PanelSettings asset
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "panelSettings",
            "Assets/UI Toolkit/PanelSettings.asset") + "\n";

        // Assign the UXML (sourceAsset = VisualTreeAsset)
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "sourceAsset",
            "Assets/UI Toolkit/PauseMenuVisualTree.uxml") + "\n";

        // Set sort order higher so it renders on top of any other UI
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "sortingOrder", "100") + "\n";

        // Add PauseManager script
        result += CoplayTools.AddComponent("PauseMenu", "PauseManager") + "\n";

        // Save InTempo scene
        result += CoplayTools.SaveScene("InTempoScene") + "\n";

        // --- LEARNING SCENE ---
        result += CoplayTools.OpenScene("Assets/LukasMadeStuff/Scenes/LearningScene.unity") + "\n";

        // Create PauseMenu GameObject
        result += CoplayTools.CreateGameObject("PauseMenu", "0,0,0") + "\n";

        // Add UIDocument component
        result += CoplayTools.AddComponent("PauseMenu", "UnityEngine.UIElements.UIDocument") + "\n";

        // Assign the PanelSettings asset
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "panelSettings",
            "Assets/UI Toolkit/PanelSettings.asset") + "\n";

        // Assign the UXML
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "sourceAsset",
            "Assets/UI Toolkit/PauseMenuVisualTree.uxml") + "\n";

        // Set sort order higher
        result += CoplayTools.SetProperty("PauseMenu", "UIDocument", "sortingOrder", "100") + "\n";

        // Add PauseManager script
        result += CoplayTools.AddComponent("PauseMenu", "PauseManager") + "\n";

        // Save Learning scene
        result += CoplayTools.SaveScene("LearningScene") + "\n";

        // Return to InTempo scene (the one user had open)
        result += CoplayTools.OpenScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity") + "\n";

        return result;
    }
}
