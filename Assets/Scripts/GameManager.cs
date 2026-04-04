using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode selectedMode;
    public SongData selectedSong;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMode(GameMode mode)
    {
        selectedMode = mode;
    }

    public void SetSong(SongData song)
    {
        selectedSong = song;
    }
}
