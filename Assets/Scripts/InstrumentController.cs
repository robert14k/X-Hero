using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentController : MonoBehaviour
{
    public bool demo = false;

    private List<KeyController> keys = new List<KeyController>();
    private SongController songController;

    private float totalScore = 0;

    void Start()
    {
        // Populate the list of keys
        foreach (KeyController key in GetComponentsInChildren<KeyController>())
        {
            keys.Add(key);
            key.SetInstrumentController(this);
        }

        keys.Sort(delegate (KeyController x, KeyController y) 
        {
            int xPitch = ConvertToPitch(x.pitch);
            int yPitch = ConvertToPitch(y.pitch);
            if (xPitch == yPitch) return 0;
            else if (xPitch < yPitch) return -1;
            else return 1;
        });

        songController = SongController.Instance;
        SongController.OnNote += OnSongNote;
        SongController.OnEarlyNote += OnEarlySongNote;
    }

    public void OnKeyHit(KeyController key)
    {
        if (songController.playMode == PlayMode.Stepped)
        {
            songController.StepByAmount(1);
        }
        else
        {
            // We're in continuous mode, do scoring
        }
    }

    private void OnSongNote(List<int> noteNumbers, List<float> noteTimes)
    {
        for (int i = 0; i < noteNumbers.Count; i++)
        {
            int noteNumber = noteNumbers[i];
            float noteTime = noteTimes[i];
            if (songController.playMode == PlayMode.Continuous && demo)
            {
                PlayNote(noteNumber);
            }
            else if (songController.playMode == PlayMode.Stepped)
            {
                PrepNote(noteNumber, noteTime, Color.blue);
            }
        }
    }

    private void OnEarlySongNote(List<int> noteNumbers, List<float> noteTimes)
    {
        if (songController.playMode == PlayMode.Stepped)
        {
            ResetNoteVisuals();
        }
        for (int i = 0; i < noteNumbers.Count; i++)
        {
            int noteNumber = noteNumbers[i];
            float noteTime = noteTimes[i];
            if (songController.playMode == PlayMode.Continuous)
            {
                PrepNote(noteNumber, noteTime, Color.blue);
            }
            else if (songController.playMode == PlayMode.Stepped)
            {
                EarlyPrepNote(noteNumber, noteTime, Color.yellow);
            }
        }
    }

    private void ResetNoteVisuals()
    {
        foreach (KeyController key in keys)
        {
            key.ResetVisuals();
        }
    }

    public void PlayNote(int note)
    {
        if (note < 0 || note > keys.Count)
        {
            return;
        }
        keys[note].Play(Color.green);
    }

    public void PrepNote(int note, float noteTime, Color color)
    {
        if (note < 0 || note > keys.Count)
        {
            return;
        }
        keys[note].Prep(color, 0.5f);
    }

    public void EarlyPrepNote(int note, float noteTime, Color color)
    {
        if (note < 0 || note > keys.Count)
        {
            return;
        }
        keys[note].EarlyPrep(color, 0.5f);
    }

    public static int ConvertToPitch(string note)
    {
        note = note.ToLower();
        string sym = "";
        int oct = 0;
        string[][] notes = new string[][] { 
            new string[] {"c"}, 
            new string[] {"db", "c#"}, 
            new string[] {"d"}, 
            new string[] {"eb", "d#"},
            new string[] {"e"},
            new string[] {"f"},
            new string[] {"gb", "f#"},
            new string[] {"g"},
            new string[] {"ab", "g#"},
            new string[] {"a"},
            new string[] {"bb", "a#"},
            new string[] {"b"} };

        string splitNote = note;

        // If the length is two, then grab the symbol and number.
        // Otherwise, it must be a two-char note.
        if (splitNote.Length == 2)
        {
            sym += splitNote[0];
            oct = splitNote[1] - '0';
        }
        else if (splitNote.Length == 3)
        {
            sym += splitNote[0];
            sym += splitNote[1];
            oct = splitNote[2] - '0';
        }

        // Find the corresponding note in the array.
        for (int i = 0; i < notes.Length; i++)
            for (int j = 0; j < notes[i].Length; j++)
            {
                if (notes[i][j].Equals(sym))
                {
                    return oct * 12 + i;
                }
            }

        // If nothing was found, we return -1.
        return -1;
    }
}
