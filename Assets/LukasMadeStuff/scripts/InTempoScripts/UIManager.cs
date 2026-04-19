using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI currentPlayingTextUI;
    public TextMeshProUGUI currentAccuracyTextUI;
    public TextMeshProUGUI currentScoreTextUI;

    public void UpdateTheAccuracyTextOnUI(float accuracyNum)
    {
        currentAccuracyTextUI.text = "Accuracy: " + accuracyNum + "%";
    }
    public void UpdateThePointTextOnUI(float pointNum)
    {
        currentScoreTextUI.text = "Score: " + pointNum;
    }

    public void UpdateTheCurrentPlayingTextOnUI(string newChord)
    {
        currentPlayingTextUI.text = "Currently Playing: " + newChord;
    }
}
