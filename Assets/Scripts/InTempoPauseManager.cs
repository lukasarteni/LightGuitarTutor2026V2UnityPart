using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Pause manager for InTempoScene.
///
/// The InTempo scene uses AudioSettings.dspTime for song timing, which does NOT
/// stop when Time.timeScale = 0. So we must:
///   1. Pause the AudioSource
///   2. Disable NoteSpawner so it stops spawning (its Update reads dspTime)
///   3. Disable HitManager so it stops culling notes as misses (reads dspTime via spawner)
///   4. Freeze all NoteScroller objects (visual movement uses Time.deltaTime)
///   5. On resume, shift NoteSpawner.startTime by the paused DSP duration
///      so songTime picks up exactly where it left off
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class InTempoPauseManager : MonoBehaviour
{
    [Header("Scene References (auto-found if left empty)")]
    public AudioSource musicSource;
    public NoteSpawner noteSpawner;
    public HitManager hitManager;

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
            noteSpawner = FindAnyObjectByType<NoteSpawner>();

        if (hitManager == null)
            hitManager = FindAnyObjectByType<HitManager>();

        // The song starts itself via NoteSpawner.Start() with PlayScheduled,
        // so we just mark that we're ready after a brief wait.
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

        // 1. Pause audio
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();

        // 2. Disable NoteSpawner — its Update() uses dspTime to spawn notes
        if (noteSpawner != null)
            noteSpawner.enabled = false;

        // 3. Disable HitManager — its Update() uses dspTime (via spawner) to cull notes
        if (hitManager != null)
            hitManager.enabled = false;

        // 4. Freeze all NoteScroller visuals (they use Time.deltaTime, but let's be safe)
        FreezeAllNotes();

        // 5. Freeze Time.deltaTime-based movement
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

        // Calculate how long we were paused in DSP time
        double pausedDuration = AudioSettings.dspTime - _pauseDspTime;

        // Shift NoteSpawner.startTime so songTime picks up where it left off.
        // NoteSpawner calculates: songTime = AudioSettings.dspTime - startTime
        // Adding pausedDuration to startTime means songTime stays continuous.
        if (noteSpawner != null)
        {
            var field = typeof(NoteSpawner).GetField("startTime",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                double currentStartTime = (double)field.GetValue(noteSpawner);
                field.SetValue(noteSpawner, currentStartTime + pausedDuration);
            }
        }

        // Re-enable NoteSpawner and HitManager
        if (noteSpawner != null)
            noteSpawner.enabled = true;

        if (hitManager != null)
            hitManager.enabled = true;

        // Unfreeze note visuals
        UnfreezeAllNotes();

        // Restore time scale
        Time.timeScale = 1f;

        // Resume audio
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
        // Re-enable everything before leaving so the scene isn't left in a broken state
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
