using UnityEngine;
using System.Collections;

public class EatSoundBehavior : MonoBehaviour {

	int currentNote = 0;
	float timer = 0f;
	float interval;
	AudioSource note;

	// Use this for initialization
	void Start () 
	{
		note = GetComponent<AudioSource>();
		Debug.Log (note.playOnAwake);
		interval = 1.5f*4;

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(timer >= 1f)
		{
			note.Stop ();
			note.time = (currentNote * interval) + 0.01f;
			Debug.Log ("PLAYYY");
			note.Play ();
			currentNote ++;
			timer = 0f;
		}
	
		timer += Time.deltaTime;
	}
}
