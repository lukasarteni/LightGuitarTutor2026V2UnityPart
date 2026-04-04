using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    [Header("Song Library")]
    public SongData[] songs;

    // Root panels
    private VisualElement _root;
    private VisualElement _mainMenuScreen;
    private VisualElement _modeSelectScreen;
    private VisualElement _songSelectScreen;
    private VisualElement _optionsScreen;

    // Song-select helpers
    private ListView _songListView;
    private Label _modeBadge;
    private int _selectedSongIndex = -1;

    // Options controls
    private Slider _masterVolume;
    private Slider _musicVolume;
    private Slider _sfxVolume;
    private Toggle _fullscreenToggle;
    private DropdownField _resolutionDropdown;

    private readonly List<VisualElement> _allPanels = new();

    // ───────────────────────────── LIFECYCLE ─────────────────────────────

    private void Awake()
    {
        // Ensure GameManager exists
        if (GameManager.Instance == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }

        _root = GetComponent<UIDocument>().rootVisualElement;

        // Query panels
        _mainMenuScreen = _root.Q<VisualElement>("main-menu-screen");
        _modeSelectScreen = _root.Q<VisualElement>("mode-select-screen");
        _songSelectScreen = _root.Q<VisualElement>("song-select-screen");
        _optionsScreen = _root.Q<VisualElement>("options-screen");

        _allPanels.Add(_mainMenuScreen);
        _allPanels.Add(_modeSelectScreen);
        _allPanels.Add(_songSelectScreen);
        _allPanels.Add(_optionsScreen);

        // Start with only main menu visible
        ShowPanel(_mainMenuScreen);

        WireMainMenuButtons();
        WireModeSelectButtons();
        WireSongSelectScreen();
        WireOptionsScreen();
    }

    // ───────────────────────────── PANEL SWITCHING ─────────────────────────────

    private void ShowPanel(VisualElement target)
    {
        foreach (var panel in _allPanels)
            panel.style.display = panel == target ? DisplayStyle.Flex : DisplayStyle.None;
    }

    // ───────────────────────────── MAIN MENU ─────────────────────────────

    private void WireMainMenuButtons()
    {
        _root.Q<Button>("play-button").clicked += () => ShowPanel(_modeSelectScreen);
        _root.Q<Button>("options-button").clicked += () =>
        {
            LoadOptionsValues();
            ShowPanel(_optionsScreen);
        };
        _root.Q<Button>("exit-button").clicked += () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };
    }

    // ───────────────────────────── MODE SELECT ─────────────────────────────

    private void WireModeSelectButtons()
    {
        _root.Q<Button>("mode-tutorial").clicked += () => SelectMode(GameMode.Tutorial);
        _root.Q<Button>("mode-slow").clicked += () => SelectMode(GameMode.SlowAndSteady);
        _root.Q<Button>("mode-live").clicked += () => SelectMode(GameMode.Live);
        _root.Q<Button>("mode-scales").clicked += () => SelectMode(GameMode.Scales);
        _root.Q<Button>("mode-back-button").clicked += () => ShowPanel(_mainMenuScreen);
    }

    private void SelectMode(GameMode mode)
    {
        GameManager.Instance.SetMode(mode);
        RefreshSongSelectHeader();
        _selectedSongIndex = -1;
        _songListView.ClearSelection();
        ShowPanel(_songSelectScreen);
    }

    // ───────────────────────────── SONG SELECT ─────────────────────────────

    private void WireSongSelectScreen()
    {
        _modeBadge = _root.Q<Label>("mode-badge");
        _songListView = _root.Q<ListView>("song-list");

        // Configure ListView
        _songListView.makeItem = MakeSongRow;
        _songListView.bindItem = BindSongRow;
        _songListView.itemsSource = songs ?? new SongData[0];
        _songListView.fixedItemHeight = 56;
        _songListView.selectionType = SelectionType.Single;

        _songListView.selectionChanged += (items) =>
        {
            _selectedSongIndex = _songListView.selectedIndex;
        };

        _root.Q<Button>("confirm-button").clicked += OnConfirmSong;
        _root.Q<Button>("song-back-button").clicked += () => ShowPanel(_modeSelectScreen);
    }

    private VisualElement MakeSongRow()
    {
        var row = new VisualElement();
        row.AddToClassList("song-row");

        var songName = new Label { name = "song-name" };
        songName.AddToClassList("song-name");

        var artist = new Label { name = "song-artist" };
        artist.AddToClassList("song-artist");

        var duration = new Label { name = "song-duration" };
        duration.AddToClassList("song-duration");

        var badge = new Label { name = "difficulty-badge" };
        badge.AddToClassList("difficulty-badge");

        row.Add(songName);
        row.Add(artist);
        row.Add(duration);
        row.Add(badge);

        return row;
    }

    private void BindSongRow(VisualElement element, int index)
    {
        if (songs == null || index < 0 || index >= songs.Length) return;

        var song = songs[index];
        element.Q<Label>("song-name").text = song.songName;
        element.Q<Label>("song-artist").text = song.artist;
        element.Q<Label>("song-duration").text = song.duration;

        var badge = element.Q<Label>("difficulty-badge");
        badge.text = song.difficulty.ToString();

        // Remove old difficulty classes
        badge.RemoveFromClassList("difficulty-easy");
        badge.RemoveFromClassList("difficulty-medium");
        badge.RemoveFromClassList("difficulty-hard");

        switch (song.difficulty)
        {
            case Difficulty.Easy:
                badge.AddToClassList("difficulty-easy");
                break;
            case Difficulty.Medium:
                badge.AddToClassList("difficulty-medium");
                break;
            case Difficulty.Hard:
                badge.AddToClassList("difficulty-hard");
                break;
        }
    }

    private void RefreshSongSelectHeader()
    {
        var mode = GameManager.Instance.selectedMode;
        string label = mode switch
        {
            GameMode.Tutorial => "Tutorial",
            GameMode.SlowAndSteady => "Slow & Steady",
            GameMode.Live => "LIVE",
            GameMode.Scales => "Scales",
            _ => mode.ToString()
        };
        _modeBadge.text = label;
    }

    private void OnConfirmSong()
    {
        if (songs == null || _selectedSongIndex < 0 || _selectedSongIndex >= songs.Length)
        {
            Debug.LogWarning("MenuManager: No song selected.");
            return;
        }

        GameManager.Instance.SetSong(songs[_selectedSongIndex]);
        SceneManager.LoadScene("GameScene");
    }

    // ───────────────────────────── OPTIONS ─────────────────────────────

    private void WireOptionsScreen()
    {
        _masterVolume = _root.Q<Slider>("master-volume");
        _musicVolume = _root.Q<Slider>("music-volume");
        _sfxVolume = _root.Q<Slider>("sfx-volume");
        _fullscreenToggle = _root.Q<Toggle>("fullscreen-toggle");
        _resolutionDropdown = _root.Q<DropdownField>("resolution-dropdown");

        // Populate resolution choices
        var resolutions = new List<string> { "1920x1080", "1600x900", "1280x720", "1024x768", "800x600" };
        _resolutionDropdown.choices = resolutions;
        _resolutionDropdown.index = 0;

        // Save on change
        _masterVolume.RegisterValueChangedCallback(evt =>
            PlayerPrefs.SetFloat("MasterVolume", evt.newValue));

        _musicVolume.RegisterValueChangedCallback(evt =>
            PlayerPrefs.SetFloat("MusicVolume", evt.newValue));

        _sfxVolume.RegisterValueChangedCallback(evt =>
            PlayerPrefs.SetFloat("SFXVolume", evt.newValue));

        _fullscreenToggle.RegisterValueChangedCallback(evt =>
        {
            PlayerPrefs.SetInt("Fullscreen", evt.newValue ? 1 : 0);
            Screen.fullScreen = evt.newValue;
        });

        _resolutionDropdown.RegisterValueChangedCallback(evt =>
        {
            PlayerPrefs.SetString("Resolution", evt.newValue);
            ApplyResolution(evt.newValue);
        });

        _root.Q<Button>("options-back-button").clicked += () =>
        {
            PlayerPrefs.Save();
            ShowPanel(_mainMenuScreen);
        };
    }

    private void LoadOptionsValues()
    {
        _masterVolume.value = PlayerPrefs.GetFloat("MasterVolume", 80f);
        _musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 80f);
        _sfxVolume.value = PlayerPrefs.GetFloat("SFXVolume", 80f);
        _fullscreenToggle.value = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        string savedRes = PlayerPrefs.GetString("Resolution", "1920x1080");
        int idx = _resolutionDropdown.choices.IndexOf(savedRes);
        _resolutionDropdown.index = idx >= 0 ? idx : 0;
    }

    private void ApplyResolution(string res)
    {
        var parts = res.Split('x');
        if (parts.Length == 2 &&
            int.TryParse(parts[0], out int w) &&
            int.TryParse(parts[1], out int h))
        {
            Screen.SetResolution(w, h, Screen.fullScreen);
        }
    }
}
