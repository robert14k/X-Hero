using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    private SongController songController;

    private Image image;

    [SerializeField] private Sprite scaleImage;
    [SerializeField] private Sprite lowCImage;
    [SerializeField] private Sprite dImage;
    [SerializeField] private Sprite eImage;
    [SerializeField] private Sprite fImage;
    [SerializeField] private Sprite gImage;
    [SerializeField] private Sprite aImage;
    [SerializeField] private Sprite bImage;
    [SerializeField] private Sprite highCImage;

    void Start()
    {
        songController = SongController.Instance;

        image = GetComponent<Image>();
    }

    void Update()
    {
        image.enabled = false;
        if (!songController.midiPath.Contains("StreamingAssets/Songs/Tutorial.mid"))
        {
            return;
        }
        image.enabled = true;

        // This might be better as an if/else-if chain
        switch (songController.noteIndex)
        {
            case 0:
                image.sprite = lowCImage;
                break;
            case 1:
                image.sprite = dImage;
                break;
            case 2:
                image.sprite = eImage;
                break;
            case 3:
                image.sprite = fImage;
                break;
            case 4:
                image.sprite = gImage;
                break;
            case 5:
                image.sprite = aImage;
                break;
            case 6:
                image.sprite = bImage;
                break;
            case 7:
                image.sprite = highCImage;
                break;
            default:
                image.sprite = scaleImage;
                break;
        }
    }
}
