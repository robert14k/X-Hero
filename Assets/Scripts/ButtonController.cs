using UnityEngine;
using System.Collections;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

public class ButtonController: MonoBehaviour
{
	public SongController songController;
	private int speed = 0;
	private float[] speeds = new float[] { 0.3F, 0.5F, 0.8F, 1.0F, 1.2F };
	public Text speedText;
	public Text songText;
	public Text typeText;
	private string[] songs = new string[] { "Mario", "Pirate", "Wii" };
	private int song = 0;
	bool isStepped = false;

	//-------------------------------------------------
	public void ChangeSpeed()
	{
		songController.speed = speeds[speed % speeds.Length];
		speedText.text = (speed % speeds.Length + 1).ToString();
		speed++;
	}

	public void ChangeSong()
	{
		songController.midiPath = songs[song % songs.Length];
		songText.text = songs[song % songs.Length];
		song++;
		songController.ResetSong();
	}

	public void ChangeType()
	{
        if(isStepped){
			songController.playMode = PlayMode.Continuous;
			typeText.text = "Continuous";
        }
        else
        {
			songController.playMode = PlayMode.Stepped;
			typeText.text = "Stepped";
		}
		isStepped = !isStepped;
		songController.ResetSong();
	}


	////-------------------------------------------------
	//public void ShowTextHints(Hand hand)
	//{
	//	if (textHintCoroutine != null)
	//	{
	//		StopCoroutine(textHintCoroutine);
	//	}
	//	textHintCoroutine = StartCoroutine(TestTextHints(hand));
	//}


	////-------------------------------------------------
	//public void DisableHints()
	//{
	//	if (buttonHintCoroutine != null)
	//	{
	//		StopCoroutine(buttonHintCoroutine);
	//		buttonHintCoroutine = null;
	//	}

	//	if (textHintCoroutine != null)
	//	{
	//		StopCoroutine(textHintCoroutine);
	//		textHintCoroutine = null;
	//	}

	//	foreach (Hand hand in Player.instance.hands)
	//	{
	//		ControllerButtonHints.HideAllButtonHints(hand);
	//		ControllerButtonHints.HideAllTextHints(hand);
	//	}
	//}


	//-------------------------------------------------
	// Cycles through all the button hints on the controller
	//-------------------------------------------------
	private IEnumerator TestButtonHints(Hand hand)
	{
		ControllerButtonHints.HideAllButtonHints(hand);

		while (true)
		{
			for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex++)
			{
				ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];
				if (action.GetActive(hand.handType))
				{
					ControllerButtonHints.ShowButtonHint(hand, action);
					yield return new WaitForSeconds(1.0f);
					ControllerButtonHints.HideButtonHint(hand, action);
					yield return new WaitForSeconds(0.5f);
				}
				yield return null;
			}

			ControllerButtonHints.HideAllButtonHints(hand);
			yield return new WaitForSeconds(1.0f);
		}
	}


	//-------------------------------------------------
	// Cycles through all the text hints on the controller
	//-------------------------------------------------
	private IEnumerator TestTextHints(Hand hand)
	{
		ControllerButtonHints.HideAllTextHints(hand);

		while (true)
		{
			for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex++)
			{
				ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];
				if (action.GetActive(hand.handType))
				{
					ControllerButtonHints.ShowTextHint(hand, action, action.GetShortName());
					yield return new WaitForSeconds(3.0f);
					ControllerButtonHints.HideTextHint(hand, action);
					yield return new WaitForSeconds(0.5f);
				}
				yield return null;
			}

			ControllerButtonHints.HideAllTextHints(hand);
			yield return new WaitForSeconds(3.0f);
		}
	}
}
