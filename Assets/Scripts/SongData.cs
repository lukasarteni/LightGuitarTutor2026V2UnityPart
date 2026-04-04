using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName = "Light Guitar Tutor/Song Data")]
public class SongData : ScriptableObject
{
    public string songName;
    public string artist;
    public string duration;
    public Difficulty difficulty;
    public AudioClip clip;
}
