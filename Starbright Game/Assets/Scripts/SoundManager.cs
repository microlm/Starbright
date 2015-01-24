using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public float minMass;
	public float maxMass;
	public int numNotes;
	public int randomIndex;

	SoundBehavior EatSounds;
	SoundBehavior EatSoundsBackup;
	SoundBehavior EatSoundsBackupBackup;
	SoundBehavior HitSounds;
	SoundBehavior MiscSounds;

	private static SoundManager instance;

	// Use this for initialization
	void Start () {
		instance = this;
		EatSounds = GameObject.Find ("Eating Sounds").GetComponent<SoundBehavior>();
		EatSoundsBackup = GameObject.Find ("Eating Sounds 2").GetComponent<SoundBehavior>();
		EatSoundsBackupBackup = GameObject.Find ("Eating Sounds 3").GetComponent<SoundBehavior>();
		HitSounds = GameObject.Find ("Hit Sounds").GetComponent<SoundBehavior>();
		MiscSounds = GameObject.Find ("Misc Sounds").GetComponent<SoundBehavior> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static SoundManager Instance
	{
		get { return instance; }
	}

	public void PlayEatSound(int note)
	{
		if (EatSounds != null)
		{
			if (EatSounds.IsPlaying && EatSoundsBackup != null)
			{
				if (!EatSoundsBackup.IsPlaying && EatSoundsBackupBackup != null)
				{
						EatSoundsBackupBackup.PlayNote (note);
				}
				else EatSoundsBackup.PlayNote (note);
			}
			else EatSounds.PlayNote (note);
		}
	}

	public void PlayHitSound(int note)
	{
		if (HitSounds != null)
			HitSounds.PlayNote (note);
	}

	public void PlayMiscSound(int misc)
	{
		if (MiscSounds != null)
			MiscSounds.PlayNote (misc);
	}

	public int GetNoteByMass(float mass) 
	{
		float percent = (maxMass - mass) / (maxMass - minMass);
		int note = Mathf.FloorToInt (percent * numNotes);
		note += Random.Range (-randomIndex, randomIndex) + 1;
		if (note > 21)
			note = 21;
		if (note < 1)
			note = 1;
		return note;
	}
}
