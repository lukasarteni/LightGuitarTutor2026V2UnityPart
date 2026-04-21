using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Manages the Song Complete screen that appears when the track finishes.
/// Attach to a GameObject with a UIDocument that uses SongCompleteVisualTree.uxml.
/// Detects when the AudioSource stops playing and shows the completion overlay.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class SongCompleteManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The AudioSource playing the song. If left null, the script will search the scene.")]
    public AudioSource musicSource;

    [Tooltip("Reference to the PauseManager so we can disable pausing when the song ends.")]
    public PauseManager pauseManager;

    [Tooltip("Reference to the GameHudManager to read final stats.")]
    public GameHudManager gameHudManager;

    private UIDocument _uiDocument;
    private VisualElement _overlay;

    // Stat labels
    private Label _songInfoLabel;
    private Label _finalScore;
    private Label _finalAccuracy;
    private Label _finalPerfects;
    private Label _finalGoods;
    private Label _finalMisses;

    private bool _songCompleted;
    private bool _songWasPlaying;

    // ───────────────────────── LIFECYCLE ─────────────────────────

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;

        _overlay = root.Q<VisualElement>("song-complete-overlay");
        _songInfoLabel = root.Q<Label>("complete-song-info");
        _finalScore = root.Q<Label>("final-score");
        _finalAccuracy = root.Q<Label>("final-accuracy");
        _finalPerfects = root.Q<Label>("final-perfects");
        _finalGoods = root.Q<Label>("final-goods");
        _finalMisses = root.Q<Label>("final-misses");

        // Ensure overlay is hidden at start
        _overlay.style.display = DisplayStyle.None;

        // Wire buttons
        root.Q<Button>("play-again-button").clicked += OnPlayAgainClicked;
        root.Q<Button>("another-song-button").clicked += OnAnotherSongClicked;
        root.Q<Button>("different-mode-button").clicked += OnDifferentModeClicked;
        root.Q<Button>("complete-main-menu-button").clicked += OnMainMenuClicked;

        // Try to find AudioSource if not assigned
        if (musicSource == null)
        {
            var audioGO = GameObject.Find("AudioSource");
            if (audioGO != null)
                musicSource = audioGO.GetComponent<AudioSource>();
        }

        // Try to find PauseManager if not assigned
        if (pauseManager == null)
        {
            pauseManager = FindAnyObjectByType<PauseManager>();
        }

        // Try to find GameHudManager if not assigned
        if (gameHudManager == null)
        {
            gameHudManager = FindAnyObjectByType<GameHudManager>();
        }
    }

    private void Update()
    {
        if (_songCompleted) return;

        if (musicSource == null) return;

        // Track whether the song has started playing
        if (musicSource.isPlaying)
        {
            _songWasPlaying = true;
        }

        // Detect when the song finishes: it was playing and now it's not,
        // and the time is near the end of the clip (not just paused)
        if (_songWasPlaying && !musicSource.isPlaying && !IsPaused())
        {
            // Check that we're actually near the end of the clip (not just stopped early)
            if (musicSource.clip != null)
            {
                float timeRemaining = musicSource.clip.length - musicSource.time;
                // If less than 1 second remaining or time has reset to 0, song is done
                if (timeRemaining < 1f || musicSource.time < 0.1f)
                {
                    OnSongComplete();
                }
            }
            else
            {
                // No clip assigned, just check if it stopped
                OnSongComplete();
            }
        }
    }

    // ───────────────────────── SONG COMPLETE ─────────────────────────

    private void OnSongComplete()
    {
        _songCompleted = true;

        // Pause the game time
        Time.timeScale = 0f;

        // Disable the pause manager so ESC doesn't open pause menu
        if (pauseManager != null)
            pauseManager.enabled = false;

        // Populate song info
        if (GameManager.Instance != null && GameManager.Instance.selectedSong != null)
        {
            var song = GameManager.Instance.selectedSong;
            _songInfoLabel.text = $"{song.songName} — {song.artist}";
        }
        else
        {
            _songInfoLabel.text = "Unknown Song";
        }

        // Populate stats (placeholder values — hook up to your scoring system)
        _finalScore.text = "—";
        _finalAccuracy.text = "—";
        _finalPerfects.text = "—";
        _finalGoods.text = "—";
        _finalMisses.text = "—";

        // Show the overlay
        _overlay.style.display = DisplayStyle.Flex;

        Debug.Log("Song Complete! Showing results screen.");
    }

    /// <summary>
    /// Call this from your scoring system to populate the final stats.
    /// </summary>
    public void SetFinalStats(int score, float accuracyPercent, int perfects, int goods, int misses)
    {
        _finalScore.text = score.ToString("N0");
        _finalAccuracy.text = $"{accuracyPercent:F1}%";
        _finalPerfects.text = perfects.ToString();
        _finalGoods.text = goods.ToString();
        _finalMisses.text = misses.ToString();
    }

    // ───────────────────────── HELPERS ─────────────────────────

    private bool IsPaused()
    {
        // If timeScale is 0 and the pause manager is active, we're paused
        return Time.timeScale == 0f;
    }

    // ───────────────────────── BUTTON CALLBACKS ─────────────────────────

    private void OnPlayAgainClicked()
    {
        Time.timeScale = 1f;
        // Reload the current scene to replay the same song
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnAnotherSongClicked()
    {
        Time.timeScale = 1f;
        // Go back to MainMenu — the MenuManager will show song select
        // We set a flag so MenuManager knows to jump to song select
        PlayerPrefs.SetString("ReturnTo", "SongSelect");
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDifferentModeClicked()
    {
        Time.timeScale = 1f;
        // Go back to MainMenu — the MenuManager will show mode select
        PlayerPrefs.SetString("ReturnTo", "ModeSelect");
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
