using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Drop onto a GameObject with a UIDocument that has the PauseMenu UXML.
/// Handles ESC → pause overlay, resume with 5-second countdown, options, and return to main menu.
/// Works with Time.timeScale so anything using Time.deltaTime is automatically paused.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class PauseManager : MonoBehaviour
{
    [Header("Audio (auto-found if left empty)")]
    [Tooltip("The AudioSource playing the song. If left null, the script will search the scene.")]
    public AudioSource musicSource;

    private UIDocument _uiDocument;
    private VisualElement _root;

    // Panels
    private VisualElement _pauseOverlay;
    private VisualElement _pauseButtons;
    private VisualElement _pauseOptionsPanel;
    private VisualElement _countdownOverlay;
    private Label _countdownLabel;

    // Options controls
    private Slider _masterVolume;
    private Slider _musicVolume;
    private Slider _sfxVolume;

    private bool _isPaused;
    private bool _isCountingDown;
    private bool _songStarted;
    private Coroutine _countdownCoroutine;

    // ───────────────────────── LIFECYCLE ─────────────────────────

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _root = _uiDocument.rootVisualElement;

        // Query elements
        _pauseOverlay = _root.Q<VisualElement>("pause-overlay");
        _pauseButtons = _root.Q<VisualElement>("pause-buttons");
        _pauseOptionsPanel = _root.Q<VisualElement>("pause-options-panel");
        _countdownOverlay = _root.Q<VisualElement>("countdown-overlay");
        _countdownLabel = _root.Q<Label>("countdown-label");

        // Hide everything at start
        _pauseOverlay.style.display = DisplayStyle.None;
        _countdownOverlay.style.display = DisplayStyle.None;

        // Wire buttons
        _root.Q<Button>("resume-button").clicked += OnResumeClicked;
        _root.Q<Button>("pause-options-button").clicked += OnOptionsClicked;
        _root.Q<Button>("main-menu-button").clicked += OnMainMenuClicked;
        _root.Q<Button>("pause-options-back").clicked += OnOptionsBackClicked;

        // Wire option controls
        _masterVolume = _root.Q<Slider>("pause-master-volume");
        _musicVolume = _root.Q<Slider>("pause-music-volume");
        _sfxVolume = _root.Q<Slider>("pause-sfx-volume");

        LoadOptionsValues();

        _masterVolume.RegisterValueChangedCallback(evt =>
            PlayerPrefs.SetFloat("MasterVolume", evt.newValue));
        _musicVolume.RegisterValueChangedCallback(evt =>
            PlayerPrefs.SetFloat("MusicVolume", evt.newValue));
        _sfxVolume.RegisterValueChangedCallback(evt =>
            PlayerPrefs.SetFloat("SFXVolume", evt.newValue));

        // Try to find AudioSource if not assigned
        if (musicSource == null)
        {
            var audioGO = GameObject.Find("AudioSource");
            if (audioGO != null)
                musicSource = audioGO.GetComponent<AudioSource>();
        }

        // Start the song with a countdown
        StartCoroutine(StartSongCountdown());
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isCountingDown)
                return; // ignore ESC during countdown

            if (_isPaused)
                OnResumeClicked();
            else
                Pause();
        }
    }

    // ───────────────────────── START COUNTDOWN ─────────────────────────

    private IEnumerator StartSongCountdown()
    {
        _isCountingDown = true;
        Time.timeScale = 0f;

        _countdownOverlay.style.display = DisplayStyle.Flex;

        for (int i = 5; i >= 1; i--)
        {
            _countdownLabel.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        _countdownOverlay.style.display = DisplayStyle.None;
        _isCountingDown = false;
        _songStarted = true;

        // Un-pause and start the music
        Time.timeScale = 1f;

        if (musicSource != null)
            musicSource.Play();
    }

    // ───────────────────────── PAUSE / RESUME ─────────────────────────

    private void Pause()
    {
        if (!_songStarted) return; // Can't pause during start countdown

        _isPaused = true;
        Time.timeScale = 0f;

        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();

        // Show pause overlay with main buttons
        _pauseButtons.style.display = DisplayStyle.Flex;
        _pauseOptionsPanel.style.display = DisplayStyle.None;
        _pauseOverlay.style.display = DisplayStyle.Flex;
    }

    private void Resume()
    {
        _isPaused = false;
        _pauseOverlay.style.display = DisplayStyle.None;

        // Start countdown before actually un-pausing
        if (_countdownCoroutine != null)
            StopCoroutine(_countdownCoroutine);

        _countdownCoroutine = StartCoroutine(CountdownAndResume());
    }

    private IEnumerator CountdownAndResume()
    {
        _isCountingDown = true;
        _countdownOverlay.style.display = DisplayStyle.Flex;

        for (int i = 5; i >= 1; i--)
        {
            _countdownLabel.text = i.ToString();
            // Use WaitForSecondsRealtime because timeScale is still 0
            yield return new WaitForSecondsRealtime(1f);
        }

        _countdownOverlay.style.display = DisplayStyle.None;
        _isCountingDown = false;

        // Un-pause the game
        Time.timeScale = 1f;

        if (musicSource != null)
            musicSource.UnPause();
    }

    // ───────────────────────── BUTTON CALLBACKS ─────────────────────────

    private void OnResumeClicked()
    {
        Resume();
    }

    private void OnOptionsClicked()
    {
        LoadOptionsValues();
        _pauseButtons.style.display = DisplayStyle.None;
        _pauseOptionsPanel.style.display = DisplayStyle.Flex;
    }

    private void OnOptionsBackClicked()
    {
        PlayerPrefs.Save();
        _pauseOptionsPanel.style.display = DisplayStyle.None;
        _pauseButtons.style.display = DisplayStyle.Flex;
    }

    private void OnMainMenuClicked()
    {
        // Restore time scale before switching scenes
        Time.timeScale = 1f;
        _isPaused = false;

        SceneManager.LoadScene("MainMenu");
    }

    // ───────────────────────── OPTIONS HELPERS ─────────────────────────

    private void LoadOptionsValues()
    {
        _masterVolume.value = PlayerPrefs.GetFloat("MasterVolume", 80f);
        _musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 80f);
        _sfxVolume.value = PlayerPrefs.GetFloat("SFXVolume", 80f);
    }
}
