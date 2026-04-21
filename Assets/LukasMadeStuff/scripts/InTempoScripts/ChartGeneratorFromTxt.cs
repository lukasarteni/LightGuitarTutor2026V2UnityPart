using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class ChartGeneratorFromTxt : MonoBehaviour
{
    public TextAsset chordFile; // drag your .txt file here in Unity
    public float CountInTime = 0f;

    public List<NoteData> GenerateChart()
    {
        List<NoteData> notes = new List<NoteData>();
        ParseFileAndGenChart(notes);

        return notes;
    }

    void ParseFileAndGenChart(List<NoteData> notes)
    {
        string[] lines = chordFile.text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Example line:
            // Chord: C-major triad, Start: 0.0, End: 2.0, Duration: 2.0

            // Extract chord + start time using regex
            Match match = Regex.Match(
                line,
                @"Chord:\s*(\w+)-(major|minor)\s*triad,\s*Start:\s*([\d\.]+)"
            );

            if (match.Success)
            {
                string root = match.Groups[1].Value; // C, G, A, etc.
                string type = match.Groups[2].Value; // major / minor
                float time = float.Parse(match.Groups[3].Value);

                // Convert to your chord format
                string chord = (type == "minor") ? root + "m" : root;

                //notes.Add(new NoteData { time = time, chord = chord });

                notes.Add(new NoteData { time = time + CountInTime, chord = chord });
            }
        }

        Debug.Log("Parsed notes: " + notes.Count);
    }
}
