using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{

    private AudioSource tone;

    // Start is called before the first frame update
    void Start()
    {
        tone = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void onCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("mallet"))
        {
            Debug.Log(Vector3.Dot(other.contacts[0].normal, Vector3.up));
            tone.Play();
        }
    }


}
