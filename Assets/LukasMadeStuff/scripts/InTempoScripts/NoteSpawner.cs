using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public AudioSource music;
    public double startDelay = 5.0; // seconds

    public LaneManager laneManager;

    public ChartGeneratorFromTxt chartGenerator;

    public float spawnAheadTime = 10f;

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

        while (nextNote < notes.Count && notes[nextNote].time <= songTime + spawnAheadTime)
        //while (nextNote < notes.Count)
        {
            laneManager.SpawnNote(notes[nextNote], songTime);

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
}
