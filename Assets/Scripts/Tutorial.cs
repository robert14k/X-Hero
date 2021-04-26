using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private SongController songController;

    void Start()
    {
        songController = SongController.Instance;
    }

    void Update()
    {
        if (!songController.midiPath.Contains("StreamingAssets/Songs/Tutorial.mid"))
        {
            // Disable any tutorial elements before returning
            return;
        }

        // We also need to ensure the tutorial can *only* be in the mode/speed that we want it to be in

        // This might be better as an if/else chain
        switch (songController.noteIndex)
        {
            case 1:
                // Display slide 1
                break;
            case 2:
                // Display slide 2
                break;
            // et cetera
            default:
                break;
        }
    }
}
