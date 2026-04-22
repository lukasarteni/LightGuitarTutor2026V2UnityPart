using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName = "Light Guitar Tutor/Song Data")]
public class SongData : ScriptableObject
{
    public string songName;
    public string artist;
    public string duration;
    public Difficulty difficulty;
    public AudioClip clip;
    public TextAsset chart;

    [Header("Scene Routing")]
    [Tooltip("The scene name to load when this song is selected. Leave empty to fall back to the default mode-based scene.")]
    public string sceneName;
}
