using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class KeyController : MonoBehaviour
{

    private AudioSource tone;
    private Material mat;
    public string pitch;

    private Hand hand;
    

    // Start is called before the first frame update
    void Start()
    {
        tone = GetComponent<AudioSource>();
        mat = GetComponent<MeshRenderer>().material;
        hand = GetComponent<Hand>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            if (other.gameObject.CompareTag("mallet"))
            {
                Debug.Log("hit");
                tone.Play();
                SetGlow(Color.green, 1f);
                hand.TriggerHapticPulse(100);
            }
        }
    }

    private void SetGlow(Color color, float duration, float intensity = 0.5f)
    {
        mat.SetColor("_GlowColor", color);
        mat.SetFloat("_GlowIntensity", intensity);
        if (duration != 0)
        {
            StartCoroutine(DimGlow(duration));
        }
    }

    private IEnumerator DimGlow(float duration)
    {
        yield return new WaitForSeconds(duration);
        // TODO: Dim over time
        mat.SetFloat("_GlowIntensity", 0);
    }
}
