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
        if (Keyboard.current.aKey.wasPressedThisFrame & Keyboard.current.zKey.wasPressedThisFrame)
            Debug.Log("Am2");
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame){
                TryHit("Am");
                Debug.Log("Am");}
            if (Keyboard.current.eKey.wasPressedThisFrame)
                TryHit("Em");
            if (Keyboard.current.dKey.wasPressedThisFrame)
                TryHit("Dm");
        }
        else
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
                TryHit("A");
            if (Keyboard.current.bKey.wasPressedThisFrame){
                TryHit("B");
                 Debug.Log("this is where B was pressed");}
            if (Keyboard.current.cKey.wasPressedThisFrame)
                TryHit("C");

            if (Keyboard.current.dKey.wasPressedThisFrame)
                TryHit("D");
            if (Keyboard.current.eKey.wasPressedThisFrame)
                TryHit("E");
            if (Keyboard.current.fKey.wasPressedThisFrame)
                TryHit("F");

            if (Keyboard.current.gKey.wasPressedThisFrame)
                TryHit("G");
        }
    }

    void TryHit(string chordSent)
    {
        
        float songTime = music.time;

        NoteData closest = null;
        float closestError = float.MaxValue;

        foreach (var note in spawner.notes)
        {
            if (note.hit)
                continue;
            if (note.chord != chordSent)
                continue;

            float error = Mathf.Abs(songTime - note.time);

            if (error < closestError)
            {
                closestError = error;
                closest = note;
            }
        }

        if (closest == null)
            return;

        if (closestError <= goodWindow)
        {
            closest.hit = true;

            if (closest.obj != null)
                Destroy(closest.obj);

            Debug.Log("HIT " + chordSent);
        }
        else
        {
            Debug.Log("MISS");
        }
    }
}
