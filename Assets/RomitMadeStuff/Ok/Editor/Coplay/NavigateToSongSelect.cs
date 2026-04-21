using UnityEngine;
using UnityEngine.UIElements;

public class NavigateToSongSelect
{
    public static string Execute()
    {
        var menuManager = GameObject.FindFirstObjectByType<MenuManager>();
        if (menuManager == null) return "MenuManager not found";

        // Ensure GameManager exists
        var gm = GameObject.FindFirstObjectByType<GameManager>();
        if (gm == null) return "GameManager not found";
        gm.SetMode(GameMode.SlowAndSteady);

        var root = menuManager.GetComponent<UIDocument>().rootVisualElement;

        // Hide all screens
        root.Q<VisualElement>("main-menu-screen").style.display = DisplayStyle.None;
        root.Q<VisualElement>("mode-select-screen").style.display = DisplayStyle.None;
        root.Q<VisualElement>("info-screen").style.display = DisplayStyle.None;
        root.Q<VisualElement>("options-screen").style.display = DisplayStyle.None;

        // Show song select
        var songScreen = root.Q<VisualElement>("song-select-screen");
        songScreen.style.display = DisplayStyle.Flex;

        // Update the mode badge
        root.Q<Label>("mode-badge").text = "Slow & Steady";

        // Check the ListView state
        var listView = root.Q<ListView>("song-list");
        var source = listView.itemsSource;
        int count = source != null ? source.Count : -1;

        // Also rebuild list to be sure
        listView.itemsSource = menuManager.songs;
        listView.Rebuild();

        return $"Song select visible. ListView itemsSource count: {count}, songs array length: {menuManager.songs?.Length ?? -1}. Rebuilt the list.";
    }
}
