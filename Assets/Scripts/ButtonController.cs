using UnityEngine;
using System.Collections;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

public class ButtonController: MonoBehaviour
{
	public SongController songController;
	public InstrumentController instrument;
	private int speed = 1;
	private float[] speeds = new float[] { 0.3F, 0.5F, 0.8F, 1.0F, 1.2F };
	public Text speedText;
	public Text songText;
	public Text typeText;
    private string[] songs = new string[] { "Tutorial", "USA", "Mario", "Pirate", "Wii", "Bad Guy", "Castle", "Dynamite", "Eye Tiger", "Jurassic Park", "Small World", "Twinkle Star", "RuleTheWorld", "SaveMe", "StayinAlive", "Star Wars 1", "Cantina" };

	private int song = 0;
	bool isStepped = false;
	bool isTutorial = true;

    //-------------------------------------------------

    public void Start()
    {
		typeText.text = songController.playMode.ToString();
		songText.text = songs[song];
		speedText.text = (speed % speeds.Length + 1).ToString();
    }

    public void ChangeSpeed()
	{
		songController.speed = speeds[speed % speeds.Length];
		speedText.text = (speed % speeds.Length + 1).ToString();
		speed++;
		instrument.ResetNoteVisuals();
		songController.ResetSong();
	}

	public void ChangeSong()
	{
        if (isTutorial)
        {
			isTutorial = false;
			typeText.text = "Stepped";

		}
		song++;
		if (songs[song % songs.Length].Equals("Tutorial"))
        {
			isTutorial = true;
			songController.playMode = PlayMode.Stepped;
			typeText.text = "Stepped";
			isStepped = !isStepped;
		}
		songController.midiPath = songs[song % songs.Length];
		songText.text = songs[song % songs.Length];
		instrument.ResetNoteVisuals();
		songController.ResetSong();
	}

	public void ChangeType()
	{
        if (isTutorial)
        {
			typeText.text = "Only Stepped!";
			return;
		}
        if(isStepped){
			songController.playMode = PlayMode.Continuous;
			typeText.text = "Continuous";
        }
        else
        {
			songController.playMode = PlayMode.Stepped;
			songController.paused = false;
			typeText.text = "Stepped";
		}
		isStepped = !isStepped;
		instrument.ResetNoteVisuals();
		songController.ResetSong();
	}
}
