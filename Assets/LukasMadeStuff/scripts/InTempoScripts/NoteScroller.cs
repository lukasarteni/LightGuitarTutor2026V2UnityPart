using UnityEngine;

/// <summary>
/// Positions a note based on music.time so that the visual position
/// is always perfectly synced with the audio clock.
/// The note sits exactly on the scoring plank when music.time == noteTime.
/// </summary>
public class NoteScroller : MonoBehaviour
{
    public float speed = 5f;

    /// <summary>The music-time at which this note should be over the scoring plank.</summary>
    [HideInInspector] public float noteTime;

    /// <summary>The X position of the scoring plank (target position).</summary>
    [HideInInspector] public float scoringPlankX;

    /// <summary>Reference to the AudioSource driving the song.</summary>
    [HideInInspector] public AudioSource music;

    const float fixedY = 1.85f;
    const float fixedZ = -7.3f;

    void Update()
    {
        if (music == null) return;

        // Position is calculated from the audio clock every frame
        // so visuals are always perfectly synced with hit detection.
        float timeUntilHit = noteTime - music.time;
        float x = scoringPlankX + timeUntilHit * speed;

        transform.position = new Vector3(x, fixedY, fixedZ);
    }
}
