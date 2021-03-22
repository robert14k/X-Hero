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

    public bool paused = false;
    public float speed = 1;
    private float startTime;
    private float songTime = 0;
    private int currentNote = 0;

    // Start is called before the first frame update
    void Start()
    {
        midiPath = "Assets\\Songs\\" + midiPath + ".mid";
        MidiFile midiFile = MidiFile.Read(midiPath);
        tempoMap = midiFile.GetTempoMap();

        notes = new List<Note>(midiFile.GetNotes());

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            songTime += Time.deltaTime * speed;
            while (true)
            {
                Note note = notes[currentNote];
                float noteTime = note.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000000f;
                if (noteTime <= songTime)
                {
                    instrument.PlayNote(note.NoteNumber);
                    currentNote++;
                    if (currentNote == notes.Count)
                    {
                        paused = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
