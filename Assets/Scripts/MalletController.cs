using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MalletController : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool ShouldReturn = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Interactable interact = GetComponent<Interactable>();
        Hand attached = interact.attachedToHand;
        if (attached != null)
        {
            //if (!ShouldReturn)
            //{
                ShouldReturn = true;
                setHand(attached);
            //}
            return;
        }
        else
        {
            if (ShouldReturn)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
                transform.SetPositionAndRotation(startPosition, startRotation);
                ShouldReturn = false;
                return;
            }
            if(transform.position.y < startPosition.y - 1)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
                transform.SetPositionAndRotation(startPosition, startRotation);
                return;
            }
        }
        //bool isAttached = attached.ObjectIsAttached(gameObject);
        //if (!isAttached)
        //{

        //    Debug.Log("HERE2");
        //    GetComponent<Transform>().SetPositionAndRotation(transform.position, transform.rotation);
        //}
        //Transform current = GetComponent<Transform>();
        //if (current.position.y >=)
    }

    void setHand(Hand hand)
    {
        Transform handTransform = hand.transform;
        Quaternion rot = handTransform.rotation * Quaternion.Euler(0, -75, 0);
        transform.SetPositionAndRotation(handTransform.position + new Vector3(.2f, 0f, 0f), rot);
        
    }

}
