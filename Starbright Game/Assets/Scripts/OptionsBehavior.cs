using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsBehavior : MonoBehaviour {

	int on;

	// Use this for initialization
	void Start () {
		GameObject toggleText = GameObject.Find ("Tutorial Text");
		Toggle toggle = gameObject.GetComponent<Toggle>();
		if(PlayerPrefs.HasKey("TutorialOn"))
		{
			on = PlayerPrefs.GetInt ("TutorialOn");
		}
		else
		{
			on = 1;
			PlayerPrefs.SetInt("TutorialOn", on);
		}

		if(on == 1)
		{
			toggleText.GetComponent<Text>().text = "Tutorial On";
			toggle.isOn = true;
		}
		else
		{
			toggleText.GetComponent<Text>().text = "Tutorial Off";
			toggle.isOn = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeText()
	{
		GameObject toggleText = GameObject.Find ("Tutorial Text");
		Toggle toggle = gameObject.GetComponent<Toggle>();

		if(toggle.isOn)
		{
			toggleText.GetComponent<Text>().text = "Tutorial On";
		}
		else
		{
			toggleText.GetComponent<Text>().text = "Tutorial Off";
		}

		if(toggle.isOn)
		{
			PlayerPrefs.SetInt("TutorialOn", 1);
		}
		else
		{
			PlayerPrefs.SetInt("TutorialOn", 0);
		}

	}
}
