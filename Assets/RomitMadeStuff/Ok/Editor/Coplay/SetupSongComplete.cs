using UnityEngine;
using Coplay.Controllers.Functions;

public class SetupSongComplete
{
    public static string Execute()
    {
        // Create the SongComplete GameObject
        var createResult = CoplayTools.CreateGameObject("SongComplete", "0,0,0");

        // Add UIDocument component
        var addDocResult = CoplayTools.AddComponent("SongComplete", "UIDocument");

        // Add SongCompleteManager component
        var addMgrResult = CoplayTools.AddComponent("SongComplete", "SongCompleteManager");

        // Set UIDocument properties
        var setPanelSettings = CoplayTools.SetProperty(
            "SongComplete", "UIDocument", "panelSettings",
            "Assets/UI Toolkit/PanelSettings.asset");

        var setSourceAsset = CoplayTools.SetProperty(
            "SongComplete", "UIDocument", "sourceAsset",
            "Assets/UI Toolkit/SongCompleteVisualTree.uxml");

        // Set sorting order to 200 (above pause menu at 100)
        var setSortOrder = CoplayTools.SetProperty(
            "SongComplete", "UIDocument", "sortingOrder", "200");

        // Save the scene
        var saveResult = CoplayTools.SaveScene("InTempoScene");

        return $"Create: {createResult}\nAddDoc: {addDocResult}\nAddMgr: {addMgrResult}\nPanelSettings: {setPanelSettings}\nSourceAsset: {setSourceAsset}\nSortOrder: {setSortOrder}\nSave: {saveResult}";
    }
}
