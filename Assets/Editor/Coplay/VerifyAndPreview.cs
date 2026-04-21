using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

public class VerifyAndPreview
{
    public static string Execute()
    {
        string result = "";

        // Open InTempoScene to verify
        var scene = EditorSceneManager.OpenScene("Assets/LukasMadeStuff/Scenes/InTempoScene.unity", OpenSceneMode.Single);
        result += "=== InTempoScene ===\n";

        var pauseGO = GameObject.Find("PauseMenu");
        if (pauseGO != null)
        {
            var doc = pauseGO.GetComponent<UIDocument>();
            var pm = pauseGO.GetComponent<PauseManager>();
            result += $"  PauseMenu: UIDocument={doc != null}, source={doc?.visualTreeAsset?.name}, sortOrder={doc?.sortingOrder}, PauseManager={pm != null}\n";

            // Show pause overlay for preview
            if (doc != null && doc.rootVisualElement != null)
            {
                var overlay = doc.rootVisualElement.Q<VisualElement>("pause-overlay");
                if (overlay != null)
                {
                    overlay.style.display = DisplayStyle.Flex;
                    result += "  Pause overlay shown for preview.\n";
                }
            }
        }
        else
        {
            result += "  PauseMenu NOT FOUND!\n";
        }

        var scGO = GameObject.Find("SongComplete");
        if (scGO != null)
        {
            var doc = scGO.GetComponent<UIDocument>();
            var scm = scGO.GetComponent<SongCompleteManager>();
            result += $"  SongComplete: UIDocument={doc != null}, source={doc?.visualTreeAsset?.name}, sortOrder={doc?.sortingOrder}, SongCompleteManager={scm != null}\n";
        }
        else
        {
            result += "  SongComplete NOT FOUND!\n";
        }

        return result;
    }
}
