using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MalletController : MonoBehaviour
{
    private Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {


        Interactable interact = GetComponent<Interactable>();
        Hand attached = interact.attachedToHand;
        if(attached == null)
        {
            Debug.Log("HERE1");
            return;
        }
        bool isAttached = attached.ObjectIsAttached(gameObject);
        if (!isAttached)
        {

            Debug.Log("HERE2");
            GetComponent<Transform>().SetPositionAndRotation(transform.position, transform.rotation);
        }
        //Transform current = GetComponent<Transform>();
        //if (current.position.y >=)
    }

}
