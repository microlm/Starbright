using UnityEngine;
using System.Collections;

public class SoundBehavior : MonoBehaviour {

	/* time between the begining of each note */
	public float interval;

	private float timer;
	private AudioSource audio;

	// Use this for initialization
	void Start () {
		timer = 0;
	}

	// Update is called once per frame
	void Update () {
		if (timer <= 0) 
		{
			Audio.Stop();
		}

		timer -= Time.deltaTime;

		/* For Debugging
		if (Input.GetKeyDown(KeyCode.Alpha1)) 
		{
			PlayNote(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) 
		{
			PlayNote(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) 
		{
			PlayNote(3);
		}
		*/
	}

	public AudioSource Audio
	{
		get 
		{
			if (audio == null)
				audio = gameObject.GetComponent<AudioSource>(); 
			return audio;
		}
	}

	public bool IsPlaying 
	{
		get { return Audio.isPlaying; }
	}

	/** Plays nth note in sound file */
	public void PlayNote (int n)
	{
		audio.Stop ();

		float offset = n * interval;
		Audio.time = offset;
		Audio.Play ();

		timer = interval - 0.05f;
	}
}
