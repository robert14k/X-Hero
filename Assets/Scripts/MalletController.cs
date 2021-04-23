using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MalletController : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rigidBody;

    private Hand hand;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hand == null && transform.position.y < startPosition.y - 1)
        {
            ResetPosition();
        }
    }

    public void OnAttachedToHand(Hand hand)
    {
        this.hand = hand;
    }

    public void OnDetachedFromHand(Hand hand)
    {
        this.hand = null;
        ResetPosition();
    }

    private void ResetPosition()
    {
        rigidBody.velocity = new Vector3(0f, 0f, 0f);
        rigidBody.angularVelocity = new Vector3(0f, 0f, 0f);
        transform.SetPositionAndRotation(startPosition, startRotation);
        return;
    }
}
