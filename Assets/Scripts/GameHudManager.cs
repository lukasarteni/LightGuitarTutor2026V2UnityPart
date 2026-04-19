using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the in-game HUD overlay via UI Toolkit.
/// Attach to a GameObject with a UIDocument that uses GameHudVisualTree.uxml.
/// Provides public methods for gameplay scripts to update score, accuracy,
/// current chord, and chord-switch delay.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class GameHudManager : MonoBehaviour
{
    [Tooltip("If true, the delay label is visible (used in Learning mode).")]
    public bool showDelayLabel = false;

    private UIDocument _uiDocument;
    private Label _accuracyLabel;
    private Label _scoreLabel;
    private Label _currentPlayingLabel;
    private Label _delayLabel;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;

        _accuracyLabel = root.Q<Label>("accuracy-label");
        _scoreLabel = root.Q<Label>("score-label");
        _currentPlayingLabel = root.Q<Label>("current-playing-label");
        _delayLabel = root.Q<Label>("delay-label");

        // Show or hide the delay label based on mode
        if (_delayLabel != null)
        {
            _delayLabel.style.display = showDelayLabel
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
    }

    // ─────────────────── PUBLIC API ───────────────────

    /// <summary>Set the score display text.</summary>
    public void SetScore(int score)
    {
        if (_scoreLabel != null)
            _scoreLabel.text = $"Score: {score}";
    }

    /// <summary>Set the accuracy display text (0–100).</summary>
    public void SetAccuracy(float accuracyPercent)
    {
        if (_accuracyLabel != null)
            _accuracyLabel.text = $"Accuracy: {accuracyPercent:F0}%";
    }

    /// <summary>Set the currently playing chord name.</summary>
    public void SetCurrentPlaying(string chordName)
    {
        if (_currentPlayingLabel != null)
            _currentPlayingLabel.text = $"Currently Playing: {chordName}";
    }

    /// <summary>Set the delay value in milliseconds (Learning mode).</summary>
    public void SetDelay(int delayMs)
    {
        if (_delayLabel != null)
            _delayLabel.text = $"Delay: {delayMs}ms";
    }
}
