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
    

    // Awake is called before Start, allowing us to initialize the key before anything else attempts to access it
    void Awake()
    {
        tone = GetComponent<AudioSource>();
        mat = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            if (other.gameObject.CompareTag("mallet"))
            {
                Play(Color.green);
                ScoreKeep();
            }
        }
    }

    public void Play(Color color)
    {
        tone.Play();
        SetGlow(color, 1f);

        instrument.OnKeyHit(this);
    }

    public void Prep(Color color, float noteTime)
    {
        SetGlow(color, -1f);
        System.Threading.Thread.Sleep(1000);
        this.noteTime = noteTime + 1f;
        Play(Color.blue);

    }

    private void ScoreKeep()
    {
        float diff = Time.deltaTime - noteTime;
        diff *= 10;
        float score = 100 - diff;
        instrument.ScoreKeeper(score);
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
            activeDim = StartCoroutine(DimGlow(duration, intensity));
        }
    }

    private IEnumerator DimGlow(float duration, float intensity)
    {
        float progress = 0;
        while (progress < duration)
        {
            mat.SetFloat("_GlowIntensity", intensity * (duration - progress) / duration);
            progress += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    public void SetInstrumentController(InstrumentController instrument)
    {
        this.instrument = instrument;
    }
}
