using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitManager : MonoBehaviour
{
    public AudioSource music;

    public NoteSpawner spawner;

    public float perfectWindow = 0.1f;
    public float goodWindow = 0.5f;

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

        if (Keyboard.current.f1Key.wasPressedThisFrame)
            TryHit("Am");
        if (Keyboard.current.f2Key.wasPressedThisFrame)
            TryHit("Bm");
        if (Keyboard.current.f3Key.wasPressedThisFrame)
            TryHit("Cm");
        if (Keyboard.current.f4Key.wasPressedThisFrame)
            TryHit("Dm");
        if (Keyboard.current.f5Key.wasPressedThisFrame)
            TryHit("Em");
        if (Keyboard.current.f6Key.wasPressedThisFrame)
            TryHit("Fm");
        if (Keyboard.current.f7Key.wasPressedThisFrame)
            TryHit("Gm");
    }

    void TryHit(string chordSent)
    {
        Debug.Log("this is the fucker you sent : " + chordSent);
        float songTime = music.time;

        NoteData target = null;

        foreach (var note in spawner.notes)
        {
            if (note.hit)
                continue;

            float timeDiff = note.time - songTime;

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
