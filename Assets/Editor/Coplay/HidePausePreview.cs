using UnityEngine;
using UnityEngine.UIElements;

public class HidePausePreview
{
    public static string Execute()
    {
        var pauseGO = GameObject.Find("PauseMenu");
        if (pauseGO == null) return "PauseMenu not found";

        var doc = pauseGO.GetComponent<UIDocument>();
        var root = doc.rootVisualElement;
        var overlay = root.Q<VisualElement>("pause-overlay");
        overlay.style.display = DisplayStyle.None;

        return "Pause overlay hidden again.";
    }
}
