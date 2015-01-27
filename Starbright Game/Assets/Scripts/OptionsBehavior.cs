using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsBehavior : MonoBehaviour {

	int on;

	// Use this for initialization
	void Start () {
		Slider slider = GameObject.Find ("Tutorial Slider").GetComponent<Slider>();

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
			gameObject.GetComponent<Text>().text = "Tutorial On";
			slider.value = 1f;
		}
		else
		{
			gameObject.GetComponent<Text>().text = "Tutorial Off";
			slider.value = 0f;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeText()
	{
		Slider slider = GameObject.Find ("Tutorial Slider").GetComponent<Slider>();
		if(slider.value == 1)
		{
			gameObject.GetComponent<Text>().text = "Tutorial On";
		}
		else
		{
			gameObject.GetComponent<Text>().text = "Tutorial Off";
		}

		if(slider.value == 1f)
		{
			PlayerPrefs.SetInt("TutorialOn", 1);
		}
		else
		{
			PlayerPrefs.SetInt("TutorialOn", 0);
		}

	}
}
