using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class KeyController : MonoBehaviour
{
    private AudioSource tone;
    private Material mat;
    public string pitch;

    private InstrumentController instrument;
    private float noteTime;
    private Coroutine activeDim;
    private Coroutine activeProgress;

    private SongController songController;

    private bool shouldPlay = false;

    // Awake is called before Start, allowing us to initialize the key before anything else attempts to access it
    void Start()
    {
        tone = GetComponent<AudioSource>();
        mat = GetComponent<MeshRenderer>().material;

        songController = SongController.Instance;
        SongController.OnNote += OnSongNote;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            if (other.gameObject.CompareTag("mallet"))
            {
                GameObject sparks = other.gameObject.transform.GetChild(3).gameObject;
                sparks.GetComponent<ParticleSystem>().Play();
                if (songController.playMode == PlayMode.Stepped && shouldPlay)
                {
                    Play(Color.green);
                    shouldPlay = false;

                    if (activeProgress != null)
                    {
                        StopCoroutine(activeProgress);
                    }
                    mat.SetFloat("_Progress", 0);
                    if (songController.CheckNotes(pitch))
                    {
                        IgniteFireMallet();
                    }
                    else
                    {
                        ExtinguishFireMallet();
                    }
                    instrument.OnKeyHit(this);
                }
                else if (songController.playMode == PlayMode.Stepped && !shouldPlay)
                {
                    if (songController.CheckNotes(pitch))
                    {
                        IgniteFireMallet();
                    }
                    else
                    {
                        ExtinguishFireMallet();
                    }
                    Play(Color.red);
                }
                else 
                {
                    if (songController.CheckNotes(pitch))
                    {
                        IgniteFireMallet();
                    }
                    else
                    {
                        ExtinguishFireMallet();
                    }
                    Play(Color.green);
                }

                
            }
        }
    }

    private void OnSongNote(List<int> noteNumbers, List<float> noteTimes)
    {
        // Do stuff
    }

    public void Play(Color color)
    {
        tone.Play();
        SetGlow(color, .5f);
    }

    public void Prep(Color color, float offset)
    {
        if (songController.playMode == PlayMode.Stepped)
        {
            shouldPlay = true;
        }
        noteTime = Time.time + offset;

        if (null != activeProgress)
        {
            StopCoroutine(activeProgress);
        }

        mat.SetColor("_ProgressColor", color);
        mat.SetFloat("_Progress", 0f);
        activeProgress = StartCoroutine(IncreaseProgress(offset / songController.speed));
    }

    public void EarlyPrep(Color color, float offset)
    {
        if (null != activeProgress)
        {
            StopCoroutine(activeProgress);
        }

        mat.SetColor("_ProgressColor", color);
        mat.SetFloat("_Progress", 0f);
        activeProgress = StartCoroutine(IncreaseProgress(offset / songController.speed));
    }

    public void ResetVisuals()
    {
        if (activeProgress != null)
        {
            StopCoroutine(activeProgress);
            shouldPlay = false;
        }
        mat.SetFloat("_Progress", 0);
    }

    public void SetGlow(Color color, float duration, float intensity = 0.5f)
    {
        mat.SetColor("_GlowColor", color);
        mat.SetFloat("_GlowIntensity", intensity);
        if (duration != 0)
        {
            if (null != activeDim)
            {
                StopCoroutine(activeDim);
            }
            activeDim = StartCoroutine(DecreaseGlow(duration, intensity));
        }
    }

    private IEnumerator DecreaseGlow(float duration, float intensity)
    {
        float progress = 0;
        while (progress < duration)
        {
            mat.SetFloat("_GlowIntensity", intensity * (duration - progress) / duration);
            progress += Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_GlowIntensity", 0f);
        yield return null;
    }

    private IEnumerator IncreaseProgress(float duration)
    {
        float progress = 0;
        while (progress < duration)
        {
            mat.SetFloat("_Progress", progress / duration);
            progress += Time.deltaTime;
            yield return null;
        }
        if (songController.playMode == PlayMode.Continuous)
        {
            mat.SetFloat("_Progress", 0f);
        }
        yield return null;
    }

    public void SetInstrumentController(InstrumentController instrument)
    {
        this.instrument = instrument;
    }

    public void IgniteFireMallet()
    {
        foreach(GameObject mallet in GameObject.FindGameObjectsWithTag("mallet"))
        {
            GameObject fire = mallet.transform.GetChild(1).gameObject;
            GameObject fire2 = mallet.transform.GetChild(2).gameObject;
            fire.GetComponent<ParticleSystem>().Play();
            fire.GetComponent<ParticleSystem>().startSpeed += .1f;
            fire2.GetComponent<ParticleSystem>().Play();
            fire2.GetComponent<ParticleSystem>().startSpeed += .1f;
        }
    }

    public void ExtinguishFireMallet()
    {
        foreach (GameObject mallet in GameObject.FindGameObjectsWithTag("mallet"))
        {
            GameObject fire = mallet.gameObject.transform.GetChild(1).gameObject;
            GameObject fire2 = mallet.gameObject.transform.GetChild(2).gameObject;
            fire.GetComponent<ParticleSystem>().Stop();
            fire.GetComponent<ParticleSystem>().startSpeed = .5f;
            fire2.GetComponent<ParticleSystem>().Stop();
            fire2.GetComponent<ParticleSystem>().startSpeed = .5f;
        }
    }
}
