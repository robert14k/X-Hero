using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class SongController : MonoBehaviour
{
    public string midiPath;
    
    [SerializeField()] private InstrumentController instrument;

    public PlayMode playMode = PlayMode.Continuous;
    public bool paused = false;
    public float speed = 1;

    private float startTime;
    private float songTime = 0;
    private int noteIndex = 0;

    private List<Note> notes;
    private TempoMap tempoMap;

    private float totalScore = 0;
    private float averageScore = 0;

    void Start()
    {
        midiPath = "Assets\\Songs\\" + midiPath + ".mid";
        MidiFile midiFile = MidiFile.Read(midiPath);
        tempoMap = midiFile.GetTempoMap();

        notes = new List<Note>(midiFile.GetNotes());

        startTime = Time.time;

        if (playMode == PlayMode.Stepped)
            StartCoroutine(StepThroughSong());
    }

    void Update()
    {
        if (!paused && playMode == PlayMode.Continuous)
        {
            songTime += Time.deltaTime * speed;

            // Find all notes that exist between the last frame and this frame and play them
            while (true)
            {
                Note note = notes[noteIndex];
                float noteTime = GetNoteTime(note);
                if (noteTime <= songTime)
                {
                    instrument.PlayNote(note.NoteNumber);
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
        }
    }

    public void StepByAmount(int amount)
    {
        int direction = Math.Sign(amount);
        Debug.Log(noteIndex);

        for (int i = 0; i < amount; i++)
        {
            Note note;
            float noteTime;

            // All notes in a chord are considered to be part of the same "step"
            do
            {
                noteIndex += direction;
                note = notes[noteIndex];
                noteTime = GetNoteTime(note);

                instrument.PlayNote(note.NoteNumber);
                songTime = noteTime;
            } 
            while (noteTime == GetNoteTime(notes[noteIndex + direction]));
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

    public void ScoreKeeper(float score)
    {
        totalScore += score;
        averageScore = totalScore / noteIndex;
    }
}

public enum PlayMode
{
    Continuous,
    Stepped
}
