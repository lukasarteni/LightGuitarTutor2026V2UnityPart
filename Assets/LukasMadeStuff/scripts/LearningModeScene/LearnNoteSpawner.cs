using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class LearnNoteSpawner : MonoBehaviour
{
    public AudioSource music;

    public TestLaneManager laneManager;

    public LearnChartGeneratorFromTxt chartGenerator;
    public LearnHitManager hitManager1;

    public float spawnAheadTime = 10f;

    public double startDelay = 10; // seconds

    public List<NoteData> notes;

    int nextNote = 0;

    double startTime = 0;
    double songTime = 0;

    void Start()
    {
        notes = chartGenerator.GenerateChart();

        startTime = AudioSettings.dspTime + startDelay; // FIXED
        Debug.Log(startTime + " " + AudioSettings.dspTime + " " + startDelay);
        music.PlayScheduled(startTime);
    }

    void Update()
    {
        songTime = AudioSettings.dspTime - startTime;

        while (
            !hitManager1.areWePaused()
            && nextNote < notes.Count
            && notes[nextNote].time <= songTime + spawnAheadTime
        )
        //while (nextNote < notes.Count)
        {
            laneManager.SpawnNote(notes[nextNote], songTime);
            //Debug.Log(notes[nextNote].chord + " " + songTime);

            nextNote++;
        }
    }

    public double getSongTime()
    {
        return songTime;
    }

    public float getSongTimeFloat()
    {
        return (float)songTime;
    }

    public void addToStartTime(double addingtime)
    {
        startTime += addingtime;
    }
}
