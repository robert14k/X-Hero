using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MalletController : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rigidBody;

    private Hand hand;
    private SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");

    private SongController songController;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        rigidBody = GetComponent<Rigidbody>();

        songController = SongController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Mallet fell off the table
        if (hand == null && transform.position.y < startPosition.y - 1)
        {
            ResetPosition();
        }

        if (hand != null && teleportAction.state)
        {
            songController.paused = !songController.paused;
        }
    }

    public void OnAttachedToHand(Hand hand)
    {
        this.hand = hand;
        ControllerButtonHints.ShowTextHint(hand, teleportAction, "Play/Pause");
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
    }
}
