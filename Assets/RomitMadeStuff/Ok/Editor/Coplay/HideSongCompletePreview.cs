using UnityEngine;
using UnityEngine.UIElements;

public class HideSongCompletePreview
{
    public static string Execute()
    {
        var songCompleteGO = GameObject.Find("SongComplete");
        if (songCompleteGO == null)
            return "Error: SongComplete GameObject not found";

        var uiDoc = songCompleteGO.GetComponent<UIDocument>();
        if (uiDoc == null)
            return "Error: UIDocument not found on SongComplete";

        var root = uiDoc.rootVisualElement;
        var overlay = root.Q<VisualElement>("song-complete-overlay");
        if (overlay != null)
            overlay.style.display = DisplayStyle.None;

        return "Song Complete overlay hidden";
    }
}
