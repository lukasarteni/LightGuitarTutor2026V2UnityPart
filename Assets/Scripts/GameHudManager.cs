using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the in-game HUD overlay via UI Toolkit.
/// Attach to a GameObject with a UIDocument that uses GameHudVisualTree.uxml.
/// Provides public methods for gameplay scripts to update score, accuracy,
/// current chord, hit feedback, and chord-switch delay.
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

    // Hit feedback labels
    private Label _perfectLabel;
    private Label _goodLabel;
    private Label _missLabel;

    // Scheduling handles for fade-out
    private IVisualElementScheduledItem _perfectFade;
    private IVisualElementScheduledItem _goodFade;
    private IVisualElementScheduledItem _missFade;

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

        _perfectLabel = root.Q<Label>("perfect-label");
        _goodLabel = root.Q<Label>("good-label");
        _missLabel = root.Q<Label>("miss-label");

        // Show or hide the delay label based on mode
        if (_delayLabel != null)
        {
            _delayLabel.style.display = showDelayLabel
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        // Ensure hit feedback labels start hidden
        HideAllFeedback();
    }

    // ─────────────────── PUBLIC API ───────────────────

    /// <summary>Set the score display text.</summary>
    public void SetScore(float score)
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

    // ─────────────────── HIT FEEDBACK ───────────────────

    /// <summary>Flash the PERFECT feedback label.</summary>
    public void ShowPerfect()
    {
        ShowFeedbackLabel(_perfectLabel);
    }

    /// <summary>Flash the GOOD feedback label.</summary>
    public void ShowGood()
    {
        ShowFeedbackLabel(_goodLabel);
    }

    /// <summary>Flash the MISS feedback label.</summary>
    public void ShowMiss()
    {
        ShowFeedbackLabel(_missLabel);
    }

    private void HideAllFeedback()
    {
        if (_perfectLabel != null) _perfectLabel.style.opacity = 0f;
        if (_goodLabel != null) _goodLabel.style.opacity = 0f;
        if (_missLabel != null) _missLabel.style.opacity = 0f;
    }

    private void ShowFeedbackLabel(Label label)
    {
        if (label == null) return;

        // Hide all first
        HideAllFeedback();

        // Remove transition temporarily so we can snap to full opacity
        label.style.transitionDuration = new StyleList<TimeValue>(
            new System.Collections.Generic.List<TimeValue> { new TimeValue(0f) });
        label.style.opacity = 1f;

        // Re-enable transition on next frame, then fade to 0
        label.schedule.Execute(() =>
        {
            label.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue> { new TimeValue(0.8f) });
            label.style.opacity = 0f;
        });
    }
}
