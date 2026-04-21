using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Coplay.Controllers.Functions;

public class SetupSongCompleteLearning
{
    public static string Execute()
    {
        // Open the LearningScene
        var openResult = CoplayTools.OpenScene("Assets/LukasMadeStuff/Scenes/LearningScene.unity");

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

        var setSortOrder = CoplayTools.SetProperty(
            "SongComplete", "UIDocument", "sortingOrder", "200");

        // Save the scene
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        bool saved = EditorSceneManager.SaveScene(scene, "Assets/LukasMadeStuff/Scenes/LearningScene.unity");

        // Re-open the InTempoScene
        var reopenResult = CoplayTools.OpenScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity");

        return $"Open: {openResult}\nCreate: {createResult}\nAddDoc: {addDocResult}\nAddMgr: {addMgrResult}\nPanelSettings: {setPanelSettings}\nSourceAsset: {setSourceAsset}\nSortOrder: {setSortOrder}\nSaved: {saved}\nReopen: {reopenResult}";
    }
}
