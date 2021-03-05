using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentController : MonoBehaviour
{
    private List<KeyController> keys = new List<KeyController>();

    void Start()
    {
        // Populate the list of keys
        foreach (Transform child in transform)
        {
            KeyController key = child.GetComponent<KeyController>();
            if (null != key)
            {
                keys.Add(key);
            }
        }
        keys.Sort(/*TODO: SORT BY PITCH*/);
    }

    public void OnKeyHit(KeyController key)
    {
        // DO STUFF OR SOMETHIN IDK
    }

    private int KeyToPitch(string key)
    {
        key = key.ToLower();
        return 0;
    }

    public static int ConvertToPitch(string note)
    {
        string sym = "";
        int oct = 0;
        string[][] notes = new string[][] { 
            new string[] {"C"}, 
            new string[] {"Db", "C#"}, 
            new string[] {"D"}, 
            new string[] {"Eb", "D#"},
            new string[] {"E"},
            new string[] {"F"},
            new string[] {"Gb", "F#"},
            new string[] {"G"},
            new string[] {"Ab", "G#"},
            new string[] {"A"},
            new string[] {"Bb", "A#"},
            new string[] {"B"} };

        string splitNote = note;
        foreach (char s in splitNote)
        {
            Debug.Log(s);
        }

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
