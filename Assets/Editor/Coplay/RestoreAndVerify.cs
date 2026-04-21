using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

public class RestoreAndVerify
{
    public static string Execute()
    {
        string result = "";

        // Re-enable Canvas
        var canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null)
        {
            // It might be inactive, search all root objects
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            foreach (var go in scene.GetRootGameObjects())
            {
                if (go.name == "Canvas")
                {
                    go.SetActive(true);
                    result += "Canvas re-enabled.\n";
                    break;
                }
            }
        }

        // Hide pause overlay (it should be hidden by default, only shown at runtime)
        var pauseGO = GameObject.Find("PauseMenu");
        if (pauseGO != null)
        {
            var doc = pauseGO.GetComponent<UIDocument>();
            if (doc != null && doc.rootVisualElement != null)
            {
                var overlay = doc.rootVisualElement.Q<VisualElement>("pause-overlay");
                if (overlay != null)
                    overlay.style.display = DisplayStyle.None;
                result += "Pause overlay hidden (will show at runtime via ESC).\n";
            }
        }

        // Save InTempoScene
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        result += "InTempoScene saved.\n";

        // Now verify LearningScene
        var scene2 = EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/LearningScene.unity", OpenSceneMode.Single);
        result += "\n=== LearningScene Verification ===\n";
        
        var pauseGO2 = GameObject.Find("PauseMenu");
        var scGO2 = GameObject.Find("SongComplete");
        
        if (pauseGO2 != null)
        {
            var doc = pauseGO2.GetComponent<UIDocument>();
            var pm = pauseGO2.GetComponent<PauseManager>();
            result += $"  PauseMenu: UIDocument={doc != null} (source={doc?.visualTreeAsset?.name}, sort={doc?.sortingOrder}), PauseManager={pm != null}\n";
        }
        else result += "  PauseMenu: MISSING!\n";

        if (scGO2 != null)
        {
            var doc = scGO2.GetComponent<UIDocument>();
            var scm = scGO2.GetComponent<SongCompleteManager>();
            result += $"  SongComplete: UIDocument={doc != null} (source={doc?.visualTreeAsset?.name}, sort={doc?.sortingOrder}), SongCompleteManager={scm != null}\n";
        }
        else result += "  SongComplete: MISSING!\n";

        // Return to MainMenu
        EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/MainMenu.unity", OpenSceneMode.Single);
        result += "\nReturned to MainMenu.\n";

        return result;
    }
}
