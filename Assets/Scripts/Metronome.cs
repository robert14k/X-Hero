using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : Singleton<Metronome>
{
    public bool playing = true;

    public float bpm;
    private float secondsPerBeat;

    private float startTime;
    private float currentTime;

    private float beatTime;
    private float lastBeatTime;

    private AudioSource beatSound;

    public void Reset()
    {
        startTime = Time.time;
        currentTime = Time.time;
        beatTime = 0;
        lastBeatTime = 0;

        secondsPerBeat = 1 / (bpm / 60f);

        beatSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        currentTime = Time.time - startTime;
        beatTime = currentTime % secondsPerBeat;
        if (lastBeatTime > beatTime && playing) // We've passed the threshold for a beat
        {
            beatSound.Play();
        }
        lastBeatTime = beatTime;
    }
}
