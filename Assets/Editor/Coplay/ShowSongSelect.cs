using UnityEngine;
using UnityEngine.UIElements;

public class ShowSongSelect
{
    public static string Execute()
    {
        var menuManager = GameObject.FindFirstObjectByType<MenuManager>();
        if (menuManager == null) return "MenuManager not found";

        var root = menuManager.GetComponent<UIDocument>().rootVisualElement;

        // Hide all screens
        root.Q<VisualElement>("main-menu-screen").style.display = DisplayStyle.None;
        root.Q<VisualElement>("mode-select-screen").style.display = DisplayStyle.None;
        root.Q<VisualElement>("info-screen").style.display = DisplayStyle.None;
        root.Q<VisualElement>("options-screen").style.display = DisplayStyle.None;

        // Show song select
        root.Q<VisualElement>("song-select-screen").style.display = DisplayStyle.Flex;

        return "Song select screen is now visible";
    }
}
