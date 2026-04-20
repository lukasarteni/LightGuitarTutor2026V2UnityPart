using UnityEngine;
using UnityEngine.UIElements;

public class PreviewPauseMenu
{
    public static string Execute()
    {
        var pauseGO = GameObject.Find("PauseMenu");
        if (pauseGO == null) return "Error: PauseMenu not found";

        var doc = pauseGO.GetComponent<UIDocument>();
        if (doc == null) return "Error: UIDocument not found";

        var root = doc.rootVisualElement;
        var overlay = root.Q<VisualElement>("pause-overlay");
        if (overlay == null) return "Error: pause-overlay not found";

        // Make it visible for preview
        overlay.style.display = DisplayStyle.Flex;

        return "Pause overlay made visible for preview.";
    }
}
