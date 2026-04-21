using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class PreviewBothMenus
{
    public static string Execute()
    {
        string result = "";

        // Show Pause overlay
        var pauseGO = GameObject.Find("PauseMenu");
        if (pauseGO != null)
        {
            var doc = pauseGO.GetComponent<UIDocument>();
            if (doc != null && doc.rootVisualElement != null)
            {
                var overlay = doc.rootVisualElement.Q<VisualElement>("pause-overlay");
                if (overlay != null)
                {
                    overlay.style.display = DisplayStyle.Flex;
                    result += "Pause overlay shown.\n";
                }
            }
        }

        // Hide SongComplete overlay (so we can screenshot pause first)
        var scGO = GameObject.Find("SongComplete");
        if (scGO != null)
        {
            var doc = scGO.GetComponent<UIDocument>();
            if (doc != null && doc.rootVisualElement != null)
            {
                var overlay = doc.rootVisualElement.Q<VisualElement>("song-complete-overlay");
                if (overlay != null)
                {
                    overlay.style.display = DisplayStyle.None;
                    result += "SongComplete overlay hidden.\n";
                }
            }
        }

        // Temporarily hide the Canvas so it doesn't block the UIDocument
        var canvasGO = GameObject.Find("Canvas");
        if (canvasGO != null)
        {
            canvasGO.SetActive(false);
            result += "Canvas hidden for preview.\n";
        }

        return result;
    }
}
