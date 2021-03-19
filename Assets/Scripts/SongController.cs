using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class SongController : MonoBehaviour
{
    public string midiPath;
    private List<Note> notes;
    private TempoMap tempoMap;

    [SerializeField()]
    private InstrumentController instrument;

    private float startTime;
    private int currentNote = 0;

    // Start is called before the first frame update
    void Start()
    {
        midiPath = "Assets\\Songs\\" + midiPath + ".mid";
        MidiFile midiFile = MidiFile.Read(midiPath);
        tempoMap = midiFile.GetTempoMap();

        notes = new List<Note>(midiFile.GetNotes());
        foreach(Note note in notes)
        {
            MetricTimeSpan time = note.TimeAs<MetricTimeSpan>(tempoMap);
        }

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float songTime = Time.time - startTime;
        while (true)
        {
            Note note = notes[currentNote];
            float noteTime = note.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000000f;
            if (noteTime <= songTime)
            {
                instrument.PlayNote(note.NoteNumber);
                currentNote++;
            }
            else
            {
                break;
            }
        }
    }
}
