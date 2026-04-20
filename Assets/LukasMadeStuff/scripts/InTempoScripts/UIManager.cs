using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI currentPlayingTextUI;
    public TextMeshProUGUI currentAccuracyTextUI;
    public TextMeshProUGUI currentScoreTextUI;
    public CanvasGroup perfectHitTextUICanvasGroup;
    public CanvasGroup goodHitTextUICanvasGroup;
    public CanvasGroup missHitTextUICanvasGroup;

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

    public void ShowThePerfectHit()
    {
        FadeInToFadeOutCanvasObject(perfectHitTextUICanvasGroup);
    }

    public void ShowTheGoodHit()
    {
        FadeInToFadeOutCanvasObject(goodHitTextUICanvasGroup);
    }

    public void ShowTheMissHit()
    {
        FadeInToFadeOutCanvasObject(missHitTextUICanvasGroup);
    }

    public void FadeInToFadeOutCanvasObject(CanvasGroup TextUICanvasGroup)
    {
        LeanTween.cancel(perfectHitTextUICanvasGroup.gameObject);
        LeanTween.cancel(goodHitTextUICanvasGroup.gameObject);
        LeanTween.cancel(missHitTextUICanvasGroup.gameObject);
        // Force all to 0 but the one we want
        perfectHitTextUICanvasGroup.alpha = 0;
        goodHitTextUICanvasGroup.alpha = 0;
        missHitTextUICanvasGroup.alpha = 0;
        TextUICanvasGroup.alpha = 1f;
        // Start slow, then speed up fade
        LeanTween.alphaCanvas(TextUICanvasGroup, 0f, 1f).setEase(LeanTweenType.easeInExpo);
    }
}
