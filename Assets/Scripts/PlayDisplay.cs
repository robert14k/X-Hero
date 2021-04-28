using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayDisplay : MonoBehaviour
{
    [SerializeField] private Sprite play;
    [SerializeField] private Sprite pause;

    [SerializeField] private float fadeDuration = 2;

    private Image image;

    private Coroutine fade;

    private SongController songController;
    private bool lastPauseState;

    private void Start()
    {
        songController = SongController.Instance;
        lastPauseState = songController.paused;

        image = GetComponent<Image>();
        image.sprite = songController.paused ? pause : play;

        CallFadeImage();
    }

    void Update()
    {
        image.enabled = (songController.playMode == PlayMode.Continuous);

        if (songController.paused != lastPauseState)
        {
            image.sprite = songController.paused ? pause : play;
            CallFadeImage();
        }

        lastPauseState = songController.paused;
    }

    void CallFadeImage()
    {
        if (fade != null)
        {
            StopCoroutine(fade);
        }
        fade = StartCoroutine(FadeImage(fadeDuration));
    }

    IEnumerator FadeImage(float duration)
    {
        float progress = 0;
        while (progress < duration)
        {
            image.color = new Color(1, 1, 1, (duration - progress) / (duration / 2));
            progress += Time.deltaTime;
            yield return null;
        }
        image.color = new Color(1, 1, 1, 0);
    }
}
