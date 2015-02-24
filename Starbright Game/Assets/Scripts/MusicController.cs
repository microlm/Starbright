using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	public AudioSource[] Tracks;

	private int CurrentTrack;

	public static MusicController Instance { get; private set; }

	// Use this for initialization
	void Start () {
		CurrentTrack = 0;
		Tracks [CurrentTrack].Play ();
		Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LevelUp()
	{
		if (CurrentTrack < Tracks.Length - 1)
		{
			float time = Tracks [CurrentTrack].time;
			Tracks [CurrentTrack].Stop ();
			CurrentTrack++;
			Tracks [CurrentTrack].time = time;
			Tracks [CurrentTrack].Play ();
		}

	}

	public void LevelDown()
	{
		if (CurrentTrack > 0)
		{
			float time = Tracks [CurrentTrack].time;
			Tracks [CurrentTrack].Stop ();
			CurrentTrack--;
			Tracks [CurrentTrack].time = time;
			Tracks [CurrentTrack].Play ();
		}
		
	}
}
