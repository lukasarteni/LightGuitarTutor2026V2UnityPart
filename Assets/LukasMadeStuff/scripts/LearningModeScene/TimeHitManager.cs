using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeHitManager : MonoBehaviour
{
    public AudioSource music;

    public NoteSpawner spawner;

    public float perfectWindow = 0.08f;
    public float goodWindow = 0.15f;

    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
            TryHit("G");

        if (Keyboard.current.eKey.wasPressedThisFrame)
            TryHit("Em");
    }

    void TryHit(string chord)
    {
        float songTime = music.time;

        NoteData closest = null;
        float closestError = float.MaxValue;

        foreach (var note in spawner.notes)
        {
            if (note.hit) continue;
            if (note.chord != chord) continue;

            float error = Mathf.Abs(songTime - note.time);

            if (error < closestError)
            {
                closestError = error;
                closest = note;
            }
        }

        if (closest == null) return;

        if (closestError <= goodWindow)
        {
            closest.hit = true;

            if (closest.obj != null)
                Destroy(closest.obj);

            Debug.Log("HIT " + chord);
        }
        else
        {
            Debug.Log("MISS");
        }
    }
}