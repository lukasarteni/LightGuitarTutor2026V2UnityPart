using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public AudioSource music;

    public LaneManager laneManager;

    public ChartGeneratorFromTxt chartGenerator;

    public float spawnAheadTime = 10f;

    public List<NoteData> notes;

    int nextNote = 0;

    void Start()
    {
        notes = chartGenerator.GenerateChart();

        // Music is started by PauseManager after the countdown
    }

    void Update()
    {
        float songTime = music.time;

        //while (nextNote < notes.Count && notes[nextNote].time <= songTime + spawnAheadTime)
        while (nextNote < notes.Count)
        {
            laneManager.SpawnNote(notes[nextNote], songTime);

            nextNote++;
        }
    }
}
