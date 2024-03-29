using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Linq;

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
    public int noteIndex = 0;
    private int earlyNoteIndex = 0;
    private Boolean mutex;
    private Boolean songStarted;

    private List<Note> notes;
    private TempoMap tempoMap;

    public delegate void SongAction(List<int> noteNumber, List<float> noteTime);
    public static event SongAction OnNote;
    public static event SongAction OnEarlyNote;

    private int streak = 0;

    void Awake()
    {
        ResetSong();
    }

    public void ResetSong()
    {
        mutex = true;
        paused = true;
        songStarted = false;
        if (!midiPath.Contains(".mid"))
        {
            midiPath = Application.streamingAssetsPath + "/Songs/" + midiPath + ".mid";
        }
        MidiFile midiFile = MidiFile.Read(midiPath);
        tempoMap = midiFile.GetTempoMap();

        notes = new List<Note>(midiFile.GetNotes());

        startTime = Time.time;
        songTime = 0 - delay;
        earlySongTime = 0;
        noteIndex = 0;
        earlyNoteIndex = 0;
        mutex = false;
    }

    void Update()
    {
        if (mutex)
        {
            return;
        }
        if (!paused && playMode == PlayMode.Continuous)
        {
            if (delay > 0)
            {
                HandleEarlyNotes();
            }
            HandleNotes();
        }

        if (playMode == PlayMode.Stepped && noteIndex == 0 && !songStarted)
        {
            noteIndex = -1;
            earlyNoteIndex = noteIndex;
            while (notes[earlyNoteIndex + 1].Time == notes[noteIndex + 1].Time)
            {
                earlyNoteIndex++;
            }
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
                if (earlyNoteIndex >= notes.Count)
                {
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
                if (noteIndex >= notes.Count)
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
        mutex = true;
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
                if (earlyNoteIndex >= notes.Count)
                {
                    break;
                }
                note = notes[earlyNoteIndex];
                noteTime = GetNoteTime(note);

                noteNumbers.Add(note.NoteNumber - noteOffset);
                noteTimes.Add(noteTime);
                earlySongTime = noteTime;
                if (earlyNoteIndex + direction >= notes.Count)
                {
                    break;
                }
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
                if (noteIndex >= notes.Count)
                {
                    paused = true;
                    break;
                }
                note = notes[noteIndex];
                noteTime = GetNoteTime(note);

                noteNumbers.Add(note.NoteNumber - noteOffset);
                noteTimes.Add(noteTime);
                songTime = noteTime;
                if (noteIndex + direction >= notes.Count)
                {
                    break;
                }
            } 
            while (noteTime == GetNoteTime(notes[noteIndex + direction]));
            OnNote(noteNumbers, noteTimes);
        }
        if (!songStarted)
        {
            songStarted = true;
        }
        mutex = false;
    }

    public void JumpToNote(int index)
    {
        noteIndex = index;
        songTime = GetNoteTime(notes[index]);
    }

    public float GetNoteTime(Note note)
    {
        return note.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000000f;
    }

    public Boolean CheckNotes(string noteName)
    {
        int note = InstrumentController.ConvertToPitch(noteName) + 48;
        if (playMode == PlayMode.Continuous)
        {
            List<Note> sameNotes = notes.Where(x => x.NoteNumber == note).ToList();
            if (sameNotes.Count > 0)
            {
                float currentTime = songTime;
                Note closest = sameNotes.Aggregate((x, y) => Math.Abs(GetNoteTime(x) - currentTime) < Math.Abs(GetNoteTime(y) - currentTime) ? x : y);
                if (Math.Abs(GetNoteTime(closest) - currentTime) < 1)
                {

                    streak++;
                    if (streak > 2)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }
        else
        {
            if(notes[noteIndex].NoteNumber == note)
            {
                streak++;
                if (streak > 2)
                {
                    return true;
                }
                return false;
            }
        }

        streak = 0;
        return false;

    }

}

public enum PlayMode
{
    Continuous,
    Stepped
}
