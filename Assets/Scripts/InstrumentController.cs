using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentController : MonoBehaviour
{
    public List<KeyController> keys = new List<KeyController>();

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

        //keys[ConvertToPitch("g#2")].SetGlow(Color.blue, 0f);
    }

    public void OnKeyHit(KeyController key)
    {
        // DO STUFF OR SOMETHIN IDK
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
