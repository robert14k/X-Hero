using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class SongController : Singleton<SongController>
{
    [SerializeField]
    public string midiPath;
    
    [SerializeField()]
    private InstrumentController instrument;
    public int noteOffset = 48;

    public PlayMode playMode = PlayMode.Continuous;
    public bool paused = false;
    public float speed = 1;
    public float delay = 1;

    private float startTime;
    private float songTime = 0;
    private float earlySongTime = 0;
    private int noteIndex = 0;
    private int earlyNoteIndex = 0;

    private List<Note> notes;
    private TempoMap tempoMap;

    public delegate void SongAction(List<int> noteNumber, List<float> noteTime);
    public static event SongAction OnNote;
    public static event SongAction OnEarlyNote;

    void Awake()
    {
        ResetSong();
    }

    public void ResetSong()
    {
        if (!midiPath.Contains(".mid"))
        {
            midiPath = Application.streamingAssetsPath + "/Songs/" + midiPath + ".mid";
        }
        MidiFile midiFile = MidiFile.Read(midiPath);
        tempoMap = midiFile.GetTempoMap();
        Metronome.Instance.bpm = tempoMap.Tempo.AtTime(0).BeatsPerMinute;
        Metronome.Instance.Reset();

        notes = new List<Note>(midiFile.GetNotes());

        startTime = Time.time;
        songTime = 0 - delay;
        earlySongTime = 0;
        noteIndex = 0;
        earlyNoteIndex = 0;
    }

    void Update()
    {
        if (!paused && playMode == PlayMode.Continuous)
        {
            if (delay > 0)
            {
                HandleEarlyNotes();
            }
            HandleNotes();
        }

        if (playMode == PlayMode.Stepped && noteIndex == 0)
        {
            noteIndex = -1;
            earlyNoteIndex = 1;
            StepByAmount(1);
        }
    }

    private void HandleEarlyNotes()
    {
        earlySongTime += Time.deltaTime * speed;

        // Find all notes that exist between the last frame and this frame and play them
        List<int> noteNumbers = new List<int>();
        List<float> noteTimes = new List<float>();
        while (true)
        {
            Note note = notes[earlyNoteIndex];
            float noteTime = GetNoteTime(note);
            if (noteTime <= earlySongTime)
            {
                noteNumbers.Add(note.NoteNumber - noteOffset);
                noteTimes.Add(noteTime);
                earlyNoteIndex++;
                if (earlyNoteIndex == notes.Count)
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
        OnEarlyNote(noteNumbers, noteTimes);
    }

    private void HandleNotes()
    {
        songTime += Time.deltaTime * speed;

        // Find all notes that exist between the last frame and this frame and play them
        List<int> noteNumbers = new List<int>();
        List<float> noteTimes = new List<float>();
        while (true)
        {
            Note note = notes[noteIndex];
            float noteTime = GetNoteTime(note);
            if (noteTime <= songTime)
            {
                noteNumbers.Add(note.NoteNumber - noteOffset);
                noteTimes.Add(noteTime);
                noteIndex++;
                if (noteIndex == notes.Count)
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
        OnNote(noteNumbers, noteTimes);
    }

    public void StepByAmount(int amount)
    {
        int direction = Math.Sign(amount);

        for (int i = 0; i < amount; i++)
        {
            Note note;
            float noteTime;

            // All notes in a chord are considered to be part of the same "step"
            List<int> noteNumbers = new List<int>();
            List<float> noteTimes = new List<float>();
            do
            {
                earlyNoteIndex += direction;
                note = notes[earlyNoteIndex];
                noteTime = GetNoteTime(note);

                noteNumbers.Add(note.NoteNumber - noteOffset);
                noteTimes.Add(noteTime);
                earlySongTime = noteTime;
            }
            while (noteTime == GetNoteTime(notes[earlyNoteIndex + direction]));
            OnEarlyNote(noteNumbers, noteTimes);
        }

        for (int i = 0; i < amount; i++)
        {
            Note note;
            float noteTime;

            // All notes in a chord are considered to be part of the same "step"
            List<int> noteNumbers = new List<int>();
            List<float> noteTimes = new List<float>();
            do
            {
                noteIndex += direction;
                note = notes[noteIndex];
                noteTime = GetNoteTime(note);

                noteNumbers.Add(note.NoteNumber - noteOffset);
                noteTimes.Add(noteTime);
                songTime = noteTime;
            } 
            while (noteTime == GetNoteTime(notes[noteIndex + direction]));
            OnNote(noteNumbers, noteTimes);
        }
    }

    public void JumpToNote(int index)
    {
        noteIndex = index;
        songTime = GetNoteTime(notes[index]);
    }

    private IEnumerator StepThroughSong()
    {
        while (true)
        {
            StepByAmount(1);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public float GetNoteTime(Note note)
    {
        return note.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000000f;
    }
}

public enum PlayMode
{
    Continuous,
    Stepped
}
