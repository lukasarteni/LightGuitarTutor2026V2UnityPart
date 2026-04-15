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

        music.Play();
    }

    void Update()
    {
        float songTime = music.time;

        while (nextNote < notes.Count &&
               notes[nextNote].time <= songTime + spawnAheadTime)
        {
            laneManager.SpawnNote(notes[nextNote], songTime);

            nextNote++;
        }
    }
}