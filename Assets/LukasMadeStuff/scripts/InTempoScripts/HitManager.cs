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
    public UIManager UIManage;

    public float perfectWindow = 1f;
    public float goodWindow = 2f;
    int currentNoteIndex = 0;
    float correctlyHitNotesNumber = 0f;

    void Update()
    {
        cullTheOldNotes();
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
        if (correctlyHitNotesNumber == 0)
        {
            UIManage.UpdateThePointTextOnUI(0);

            if (currentNoteIndex == 0)
            {
                UIManage.UpdateTheAccuracyTextOnUI(100);
            }
            else
            {
                UIManage.UpdateTheAccuracyTextOnUI(0);
            }
        }
        else
        {
            UIManage.UpdateThePointTextOnUI(correctlyHitNotesNumber * 100);
            UIManage.UpdateTheAccuracyTextOnUI(correctlyHitNotesNumber / currentNoteIndex * 100);
        }
    }

    void cullTheOldNotes()
    {
        float SongTimeForTargetChecks = music.time - 2.5f;

        while (currentNoteIndex < spawner.notes.Count)
        {
            var note = spawner.notes[currentNoteIndex];
            float timeDiff = note.time - SongTimeForTargetChecks;

            if (timeDiff < (0.5f - goodWindow))
            {
                note.hit = true;
                if (note.obj != null)
                    Destroy(note.obj);
                Debug.Log("MISS (too late): " + note.chord);
                currentNoteIndex++;
                UIManage.ShowTheMissHit();
            }
            else
                break;
        }
    }

    void TryHit(string chordSent)
    {
        //IMPORTANT probably need to sync music with visuals by 2.5
        float SongTimeForTargetChecks = music.time - 2.1f;
        UIManage.UpdateTheCurrentPlayingTextOnUI(chordSent + " " + SongTimeForTargetChecks); //changelater

        NoteData target = null;
        /*
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
        */
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
            correctlyHitNotesNumber++;
            Debug.Log("PERFECT " + chordSent + " " + error);
            target.hit = true;
            UIManage.ShowThePerfectHit();
        }
        else if (error <= goodWindow)
        {
            correctlyHitNotesNumber += 0.9f;
            Debug.Log("GOOD " + chordSent + " " + error);
            target.hit = true;
            UIManage.ShowTheGoodHit();
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
}
