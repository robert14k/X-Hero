using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public Transform ghostNotePrefab;
    public Vector3 travelOffset;
    public float travelDuration = 1;

    private List<KeyController> keys = new List<KeyController>();
    private SongController songController;

    void Start()
    {
        // Populate the list of keys
        foreach (KeyController key in GetComponentsInChildren<KeyController>())
        {
            keys.Add(key);
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
        SongController.OnEarlyNote += OnEarlySongNote;
    }

    private void OnEarlySongNote(List<int> noteNumbers, List<float> noteTimes)
    {
        if (songController.playMode == PlayMode.Stepped)
        {
            return;
        }
        for (int i = 0; i < noteNumbers.Count; i++)
        {
            if (noteNumbers[i] < 0 || noteNumbers[i] >= keys.Count)
            {
                continue;
            }
            Vector3 start = keys[noteNumbers[i]].transform.position;
            GhostNote ghostNote = Instantiate(ghostNotePrefab, start, Quaternion.identity).GetComponent<GhostNote>();
            // Set its start position
            ghostNote.startPos = ghostNote.transform.position;
            // Set its end position
            ghostNote.endPos = ghostNote.startPos + travelOffset;
            ghostNote.duration = songController.delay / songController.speed;

            StartCoroutine(ghostNote.MoveNote());
        }
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
