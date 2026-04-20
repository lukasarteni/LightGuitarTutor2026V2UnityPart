using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitManager : MonoBehaviour
{
    public AudioSource music;

    public NoteSpawner spawner;

    /// <summary>
    /// Reference to the UI Toolkit based GameHudManager for score, accuracy, and hit feedback.
    /// </summary>
    public GameHudManager gameHud;

    public float perfectWindow = 1f;
    public float goodWindow = 2f;
    int currentNoteIndex = 0;
    float correctlyHitNotesNumber = 0f;

    void Update()
    {
        CullOldNotes();
        UpdateTheAccuracyAndScore();

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            TryHit("A");
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            TryHit("B");
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            TryHit("C");
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            TryHit("D");
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
            TryHit("E");
        if (Keyboard.current.digit6Key.wasPressedThisFrame)
            TryHit("F");
        if (Keyboard.current.digit7Key.wasPressedThisFrame)
            TryHit("G");
        if (Keyboard.current.qKey.wasPressedThisFrame)
            TryHit("Am");
        if (Keyboard.current.wKey.wasPressedThisFrame)
            TryHit("Bm");
        if (Keyboard.current.eKey.wasPressedThisFrame)
            TryHit("Cm");
        if (Keyboard.current.rKey.wasPressedThisFrame)
            TryHit("Dm");
        if (Keyboard.current.tKey.wasPressedThisFrame)
            TryHit("Em");
        if (Keyboard.current.yKey.wasPressedThisFrame)
            TryHit("Fm");
        if (Keyboard.current.uKey.wasPressedThisFrame)
            TryHit("Gm");
    }

    void UpdateTheAccuracyAndScore()
    {
        if (gameHud == null) return;

        if (correctlyHitNotesNumber == 0)
        {
            gameHud.SetScore(0);
            gameHud.SetAccuracy(currentNoteIndex == 0 ? 100 : 0);
        }
        else
        {
            gameHud.SetScore(correctlyHitNotesNumber * 100);
            gameHud.SetAccuracy(correctlyHitNotesNumber / currentNoteIndex * 100);
        }
    }

    /// <summary>
    /// Automatically marks notes as MISS and destroys them once they've scrolled
    /// past the scoring plank beyond the good-hit window.
    /// </summary>
    void CullOldNotes()
    {
        float songTime = music.time;

        while (currentNoteIndex < spawner.notes.Count)
        {
            var note = spawner.notes[currentNoteIndex];

            // Note is in the past if songTime is well beyond note.time
            if (songTime - note.time > goodWindow)
            {
                note.hit = true;
                if (note.obj != null)
                    Destroy(note.obj);
                Debug.Log("MISS (too late): " + note.chord);
                currentNoteIndex++;
                if (gameHud != null) gameHud.ShowMiss();
            }
            else
            {
                break;
            }
        }
    }

    void TryHit(string chordSent)
    {
        float songTime = music.time;

        if (gameHud != null)
            gameHud.SetCurrentPlaying(chordSent);

        NoteData target = null;

        // Search a small window ahead from the current note index
        for (int i = currentNoteIndex;
             i < Mathf.Min(currentNoteIndex + 10, spawner.notes.Count);
             i++)
        {
            var note = spawner.notes[i];

            if (note.hit)
                continue;

            float timeDiff = note.time - songTime;

            // Note is too far in the future — stop searching
            if (timeDiff > goodWindow)
                break;

            // Wrong chord — skip
            if (note.chord != chordSent)
                continue;

            // Note is within the hit window (could be slightly early or late)
            if (Mathf.Abs(timeDiff) <= goodWindow)
            {
                target = note;
                break;
            }
        }

        if (target == null)
        {
            Debug.Log("MISS (no matching chord in window)");
            return;
        }

        // Evaluate timing
        float error = Mathf.Abs(songTime - target.time);

        if (error <= perfectWindow)
        {
            correctlyHitNotesNumber++;
            Debug.Log("PERFECT " + chordSent + " (error: " + error.ToString("F3") + "s)");
            target.hit = true;
            if (gameHud != null) gameHud.ShowPerfect();
        }
        else if (error <= goodWindow)
        {
            correctlyHitNotesNumber += 0.9f;
            Debug.Log("GOOD " + chordSent + " (error: " + error.ToString("F3") + "s)");
            target.hit = true;
            if (gameHud != null) gameHud.ShowGood();
        }
        else
        {
            Debug.Log("MISS " + chordSent + " (error: " + error.ToString("F3") + "s)");
            return;
        }

        // Destroy the note's visual object
        if (target.obj != null)
            Destroy(target.obj);

        // Advance past all consumed (hit) notes
        while (currentNoteIndex < spawner.notes.Count && spawner.notes[currentNoteIndex].hit)
        {
            currentNoteIndex++;
        }
    }
}
