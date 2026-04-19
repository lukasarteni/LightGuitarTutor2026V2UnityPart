using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class HitManager : MonoBehaviour
{
    public TextMeshProUGUI currentPlayingTextUI;

    public AudioSource music;

    public NoteSpawner spawner;

    public float perfectWindow = 1f;
    public float goodWindow = 2f;
    int currentNoteIndex = 0;

    void Update()
    {
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

    void UpdateTheCurrentPlayingTextOnUI(string newChord)
    {
        currentPlayingTextUI.text = "Currently Playing: " + newChord;
    }

    void TryHit(string chordSent)
    {
        //IMPORTANT probably need to sync music with visuals by 2.5
        float SongTimeForTargetChecks = music.time - 2.5f;
        UpdateTheCurrentPlayingTextOnUI(chordSent + " " + SongTimeForTargetChecks); //changelater

        NoteData target = null;

        // Step 1: Advance index past missed notes
        while (currentNoteIndex < spawner.notes.Count)
        {
            var note = spawner.notes[currentNoteIndex];
            float timeDiff = note.time - SongTimeForTargetChecks;

            if (timeDiff < -goodWindow)
            {
                note.hit = true;
                if (note.obj != null)
                    Destroy(note.obj);
                Debug.Log("MISS (too late): " + note.chord);
                currentNoteIndex++;
            }
            else
                break;
        }

        // Step 2: Search a small window ahead (with bounds check)
        for (
            int i = currentNoteIndex;
            i < Mathf.Min(currentNoteIndex + 5, spawner.notes.Count);
            i++
        )
        {
            var note = spawner.notes[i];

            if (note.hit)
                continue;

            float timeDiff = note.time - SongTimeForTargetChecks;
            if (timeDiff > goodWindow)
                break;
            if (note.chord != chordSent)
                continue;

            target = note;
            break;
        }

        if (target == null)
        {
            Debug.Log("MISS (no matching chord in window)");
            return;
        }

        // Step 3: Evaluate timing
        float error = Mathf.Abs(SongTimeForTargetChecks - target.time);

        if (error <= perfectWindow)
        {
            Debug.Log("PERFECT " + chordSent + " " + error);
            target.hit = true;
        }
        else if (error <= goodWindow)
        {
            Debug.Log("GOOD " + chordSent + " " + error);
            target.hit = true;
        }
        else
        {
            Debug.Log("MISS");
            return;
        }

        if (target.obj != null)
            Destroy(target.obj);

        // Advance past all consumed (hit) notes
        while (currentNoteIndex < spawner.notes.Count && spawner.notes[currentNoteIndex].hit)
        {
            currentNoteIndex++;
        }
    }

    /*void TryHit(string chordSent)
        {
            //Debug.Log("this is the fucker you sent : " + chordSent);
            UpdateTheCurrentPlayingTextOnUI(chordSent);
            float songTime = music.time;
    
            NoteData target = null;
    
            foreach (var note in spawner.notes)
            {
                if (note.hit)
                    continue;
    
                //float timeDiff = note.time - songTime;
                float timeDiff = 2.1f;
    
                // 🟣 Stop if future notes are too far
                if (timeDiff > goodWindow)
                    break;
    
                // 🔴 Clean up old notes (regardless of chord)
                if (-timeDiff > goodWindow)
                {
                    note.hit = true;
    
                    if (note.obj != null)
                        Destroy(note.obj);
    
                    Debug.Log("MISS (too late): " + note.chord);
                    continue;
                }
    
                if (note.chord != chordSent)
                    continue;
    
                target = note;
                break;
            }
    
            if (target == null)
            {
                Debug.Log("MISS (no matching chord in window)");
                return;
            }
    
            float error = Mathf.Abs(songTime - target.time);
    
            if (error <= perfectWindow)
            {
                Debug.Log("PERFECT " + chordSent);
            }
            else if (error <= goodWindow)
            {
                Debug.Log("GOOD " + chordSent);
            }
            else
            {
                Debug.Log("MISS");
                return;
            }
    
            target.hit = true;
    
            if (target.obj != null)
                Destroy(target.obj);
        }
    }
    */
}
