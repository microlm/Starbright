using UnityEngine;
using System.Collections;

public class KillSwitch : MonoBehaviour {

	void Awake()
	{
		int month = System.DateTime.Now.Month;
		int day = System.DateTime.Now.Day;
		int year = System.DateTime.Now.Year;

		if(month > 2 && day > 1 && year >= 2015)
		{
			Application.Quit ();
		}

	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
