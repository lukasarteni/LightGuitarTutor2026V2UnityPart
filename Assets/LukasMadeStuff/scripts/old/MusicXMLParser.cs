using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MusicXMLParser
{
    public static List<NoteData> Parse(string filePath, float bpm)
    {
        List<NoteData> notes = new List<NoteData>();

        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);

        XmlNodeList harmonyNodes = doc.GetElementsByTagName("harmony");

        float secondsPerBeat = 60f / bpm;

        int beatIndex = 0;

        foreach (XmlNode node in harmonyNodes)
        {
            string root = node["root"]["root-step"].InnerText;

            string chord = root;

            if (node["kind"] != null)
            {
                string kind = node["kind"].InnerText;

                if (kind.Contains("minor"))
                    chord += "m";
            }

            NoteData note = new NoteData();

            note.time = beatIndex * secondsPerBeat;
            note.chord = chord;

            notes.Add(note);

            beatIndex++;
        }

        return notes;
    }
}