using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayDisplay : MonoBehaviour
{
    [SerializeField] private Sprite play;
    [SerializeField] private Sprite pause;

    private Image image;

    private SongController songController;

    private void Start()
    {
        image = GetComponent<Image>();

        songController = SongController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        image.enabled = (songController.playMode == PlayMode.Continuous);

        image.sprite = songController.paused ? pause : play;

    }
}
