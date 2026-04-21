using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Pause manager for LearningScene.
///
/// Like InTempo, the Learning scene uses AudioSettings.dspTime for timing.
/// LearnHitManager also has its own note-level pause (waits for correct chord).
/// This ESC-pause must:
///   1. Pause the AudioSource
///   2. Disable LearnNoteSpawner (stops spawning, its Update reads dspTime)
///   3. Disable LearnHitManager (stops culling/input processing)
///   4. Freeze NoteScroller and TargetMover objects
///   5. On resume, shift startTime by paused DSP duration
///   6. If LearnHitManager was already in its own pause, restore that state
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class LearningPauseManager : MonoBehaviour
{
    [Header("Scene References (auto-found if left empty)")]
    public AudioSource musicSource;
    public LearnNoteSpawner noteSpawner;
    public LearnHitManager hitManager;
    public GameObject guitarMap3dModel;

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
    private double _pauseDspTime;
    private bool _hitManagerWasPaused;

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

        // Auto-find references
        if (musicSource == null)
        {
            var audioGO = GameObject.Find("AudioSource");
            if (audioGO != null)
                musicSource = audioGO.GetComponent<AudioSource>();
        }

        if (noteSpawner == null)
            noteSpawner = FindAnyObjectByType<LearnNoteSpawner>();

        if (hitManager == null)
            hitManager = FindAnyObjectByType<LearnHitManager>();

        if (guitarMap3dModel == null)
        {
            var go = GameObject.Find("3d guitar map");
            if (go != null)
                guitarMap3dModel = go;
        }

        StartCoroutine(WaitForSongStart());
    }

    private IEnumerator WaitForSongStart()
    {
        _songStarted = false;
        if (noteSpawner != null)
            yield return new WaitForSecondsRealtime((float)noteSpawner.startDelay + 0.5f);
        else
            yield return new WaitForSecondsRealtime(6f);
        _songStarted = true;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isCountingDown)
                return;

            if (_isPaused)
                OnResumeClicked();
            else
                Pause();
        }
    }

    // ───────────────────────── PAUSE / RESUME ─────────────────────────

    private void Pause()
    {
        if (!_songStarted) return;

        _isPaused = true;
        _pauseDspTime = AudioSettings.dspTime;

        // Remember if LearnHitManager was already in its own chord-wait pause
        _hitManagerWasPaused = hitManager != null && hitManager.areWePaused();

        // 1. Pause audio (if not already paused by LearnHitManager)
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();

        // 2. Disable NoteSpawner — its Update() uses dspTime
        if (noteSpawner != null)
            noteSpawner.enabled = false;

        // 3. Disable HitManager — its Update() uses dspTime via spawner
        if (hitManager != null)
            hitManager.enabled = false;

        // 4. Freeze NoteScroller visuals
        FreezeAllNotes();

        // 5. Freeze TargetMover
        if (guitarMap3dModel != null)
        {
            var mover = guitarMap3dModel.GetComponentInChildren<TargetMover>();
            if (mover != null)
                mover.FreezeAllThisNote();
        }

        // 6. Freeze Time.deltaTime-based movement
        Time.timeScale = 0f;

        // Show pause overlay
        _pauseButtons.style.display = DisplayStyle.Flex;
        _pauseOptionsPanel.style.display = DisplayStyle.None;
        _pauseOverlay.style.display = DisplayStyle.Flex;
    }

    private void Resume()
    {
        _isPaused = false;
        _pauseOverlay.style.display = DisplayStyle.None;

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
            yield return new WaitForSecondsRealtime(1f);
        }

        _countdownOverlay.style.display = DisplayStyle.None;
        _isCountingDown = false;

        // Compensate DSP time drift
        double pausedDuration = AudioSettings.dspTime - _pauseDspTime;

        if (noteSpawner != null)
            noteSpawner.addToStartTime(pausedDuration);

        // Restore time scale
        Time.timeScale = 1f;

        // Re-enable NoteSpawner
        if (noteSpawner != null)
            noteSpawner.enabled = true;

        // Re-enable HitManager
        if (hitManager != null)
            hitManager.enabled = true;

        // If LearnHitManager was already in its own chord-wait pause,
        // keep notes frozen and audio paused — let LearnHitManager handle unpausing
        // when the player hits the correct chord.
        if (_hitManagerWasPaused)
            yield break;

        // Otherwise, unfreeze everything
        UnfreezeAllNotes();

        if (guitarMap3dModel != null)
        {
            var mover = guitarMap3dModel.GetComponentInChildren<TargetMover>();
            if (mover != null)
                mover.UnfreezeAllThisNote();
        }

        if (musicSource != null)
            musicSource.UnPause();
    }

    // ───────────────────────── FREEZE / UNFREEZE NOTES ─────────────────────────

    private void FreezeAllNotes()
    {
        var scrollers = FindObjectsByType<NoteScroller>(FindObjectsSortMode.None);
        foreach (var scroller in scrollers)
            scroller.FreezeAllThisNote();
    }

    private void UnfreezeAllNotes()
    {
        var scrollers = FindObjectsByType<NoteScroller>(FindObjectsSortMode.None);
        foreach (var scroller in scrollers)
            scroller.UnfreezeAllThisNote();
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
        if (noteSpawner != null) noteSpawner.enabled = true;
        if (hitManager != null) hitManager.enabled = true;
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
