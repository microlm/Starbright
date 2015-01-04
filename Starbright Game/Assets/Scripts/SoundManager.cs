﻿using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public float minMass;
	public float maxMass;
	public int numNotes;
	public int randomIndex;

	SoundBehavior EatSounds;
	SoundBehavior HitSounds;

	private static SoundManager instance;

	// Use this for initialization
	void Start () {
		instance = this;
		EatSounds = GameObject.Find ("Eating Sounds").GetComponent<SoundBehavior>();
		HitSounds = GameObject.Find ("Hit Sounds").GetComponent<SoundBehavior>();
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
			EatSounds.PlayNote (note);
	}

	public void PlayHitSound(int note)
	{
		if (HitSounds != null)
			HitSounds.PlayNote (note);
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