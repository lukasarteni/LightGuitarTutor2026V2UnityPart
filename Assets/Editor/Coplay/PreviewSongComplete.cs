using UnityEngine;
using UnityEngine.UIElements;

public class PreviewSongComplete
{
    public static string Execute()
    {
        // Find the SongComplete UIDocument
        var songCompleteGO = GameObject.Find("SongComplete");
        if (songCompleteGO == null)
            return "Error: SongComplete GameObject not found";

        var uiDoc = songCompleteGO.GetComponent<UIDocument>();
        if (uiDoc == null)
            return "Error: UIDocument not found on SongComplete";

        var root = uiDoc.rootVisualElement;
        var overlay = root.Q<VisualElement>("song-complete-overlay");
        if (overlay == null)
            return "Error: song-complete-overlay not found";

        // Show the overlay for preview
        overlay.style.display = DisplayStyle.Flex;

        // Set some sample data
        var songInfo = root.Q<Label>("complete-song-info");
        if (songInfo != null) songInfo.text = "About A Girl — Nirvana";

        var score = root.Q<Label>("final-score");
        if (score != null) score.text = "12,450";

        var accuracy = root.Q<Label>("final-accuracy");
        if (accuracy != null) accuracy.text = "87.3%";

        var perfects = root.Q<Label>("final-perfects");
        if (perfects != null) perfects.text = "42";

        var goods = root.Q<Label>("final-goods");
        if (goods != null) goods.text = "18";

        var misses = root.Q<Label>("final-misses");
        if (misses != null) misses.text = "7";

        return "Song Complete overlay shown with sample data";
    }
}
