using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LearnHitManager : MonoBehaviour
{
    public AudioSource music;
    public LearnNoteSpawner spawner;
    public UIManager UIManage;

    //public TargetMover targetMover;

    public float perfectWindow = 1f;
    public float goodWindow = 2f;

    //public float zeroingTheCenterPieceDistance = 0.5f;
    public GameObject GuitarMap3dModelParentWithTargetMoverScript;

    int currentNoteIndex = 0;
    float correctlyHitNotesNumber = 0f;

    bool isPaused = false;
    double pauseDspTime = 0;
    NoteData pendingNote = null;

    void Update()
    {
        if (!isPaused)
            CullTheNotesThatHaveReachedTimecurrent();

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
    public Boolean areWePaused()
    {
        return isPaused;
    }
    void PauseForInput(NoteData note)
    {
        isPaused = true;
        pendingNote = note;
        music.Pause();
        pauseDspTime = AudioSettings.dspTime;

        for (int i = 0; i < spawner.notes.Count; i++)
        {
            if (spawner.notes[i].obj != null)
                spawner.notes[i].obj.GetComponent<NoteScroller>()?.FreezeAllThisNote();
        }
        GuitarMap3dModelParentWithTargetMoverScript
            .GetComponent<TargetMover>()
            ?.FreezeAllThisNote();
    }

    void Unpause()
    {
        isPaused = false;
        pendingNote = null;
        music.UnPause();
        double pausedDuration = AudioSettings.dspTime - pauseDspTime;

        // Shift startTime forward so songTime stays continuous
        spawner.addToStartTime(pausedDuration);

        

        for (int i = 0; i < spawner.notes.Count; i++)
        {
            if (spawner.notes[i].obj != null)
                spawner.notes[i].obj.GetComponent<NoteScroller>()?.UnfreezeAllThisNote();
        }
        GuitarMap3dModelParentWithTargetMoverScript
            .GetComponent<TargetMover>()
            ?.UnfreezeAllThisNote();
    }

    void UpdateTheAccuracyAndScore()
    {
        if (correctlyHitNotesNumber == 0)
        {
            UIManage.UpdateThePointTextOnUI(0);
            UIManage.UpdateTheAccuracyTextOnUI(currentNoteIndex == 0 ? 100 : 0);
        }
        else
        {
            UIManage.UpdateThePointTextOnUI(correctlyHitNotesNumber * 100);
            UIManage.UpdateTheAccuracyTextOnUI(correctlyHitNotesNumber / currentNoteIndex * 100);
        }
    }

    void CullTheNotesThatHaveReachedTimecurrent()
    {
        float SongTimeForTargetChecks = spawner.getSongTimeFloat(); //- 2.5f;

        while (currentNoteIndex < spawner.notes.Count)
        {
            var note = spawner.notes[currentNoteIndex];
            float timeDiff = note.time - SongTimeForTargetChecks;

            if (timeDiff < 0) //+ zeroingTheCenterPieceDistance)
            {
                note.hit = true;

                currentNoteIndex++;
                Debug.Log("PAUSING for chord: " + note.chord);
                PauseForInput(note);
                //UIManage.ShowTheMissHit();
            }
            else
                break;
        }
    }

    void TryHit(string chordSent)
    {
        if (isPaused)
        {
            if (pendingNote != null && chordSent == pendingNote.chord)
            {
                correctlyHitNotesNumber++;
                Debug.Log("CORRECT (after pause): " + chordSent);
                pendingNote.hit = true;
                if (pendingNote.obj != null)
                    Destroy(pendingNote.obj);
                UIManage.ShowThePerfectHit();

                while (
                    currentNoteIndex < spawner.notes.Count && spawner.notes[currentNoteIndex].hit
                )
                    currentNoteIndex++;

                Unpause();
            }
            else
            {
                Debug.Log("WRONG chord while paused, still waiting for: " + pendingNote?.chord);
            }
            return;
        }

        float SongTimeForTargetChecks = spawner.getSongTimeFloat(); //- 2.1f;
        UIManage.UpdateTheCurrentPlayingTextOnUI(chordSent + " " + SongTimeForTargetChecks);

        NoteData target = null;

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

        if (target.obj != null)
            Destroy(target.obj);

        while (currentNoteIndex < spawner.notes.Count && spawner.notes[currentNoteIndex].hit)
            currentNoteIndex++;
    }
}
