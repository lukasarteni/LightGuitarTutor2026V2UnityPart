using System.Collections.Generic;
using UnityEngine;

public class TestChartGenerator : MonoBehaviour
{
    public List<NoteData> GenerateChart()
    {
        List<NoteData> notes = new List<NoteData>();

        notes.Add(new NoteData { time = 2f, chord = "G" });
        notes.Add(new NoteData { time = 4f, chord = "G" });
        notes.Add(new NoteData { time = 6f, chord = "G" });
        notes.Add(new NoteData { time = 8f, chord = "G" });
        notes.Add(new NoteData { time = 10f, chord = "Em" });
        notes.Add(new NoteData { time = 14f, chord = "Em" });

        return notes;
    }
}