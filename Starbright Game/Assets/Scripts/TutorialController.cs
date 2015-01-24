using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialController : MonoBehaviour {

	public AnimationCurve colorTransisition;
	public Color showColor;
	public Color hideColor;

	public TutorialEvent[] EventList;

	private float timer;
	private float eventTime;
	private static TutorialController instance;

	// Use this for initialization
	void Start () 
	{
		instance = this;
		timer = 0f;
		eventTime = 0f;
		TextColor = hideColor;
		Text = "Default";
		ShowEvent("Tutorial Event Tutorial");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (timer > 0) 
		{
			timer -= Time.deltaTime;
			TextColor = Color.Lerp(hideColor, showColor, colorTransisition.Evaluate(timer/eventTime));
		}
	}

	public static TutorialController Instance
	{
		get { return instance; }
	}

	public string Text
	{
		get { return GetComponent<Text> ().text; }
		set { GetComponent<Text> ().text = value; }
	}

	public Color TextColor
	{
		get { return GetComponent<Text> ().color; }
		set { GetComponent<Text> ().color = value; }
	}

	/** Displays string text in tutorial overlay for time specified */
	public void DisplayText(string text, float time)
	{
		Text = text;
		eventTime = time;
		timer = time;
	}

	/** Displays event x and returns the event. If there is no event x, it return false */
	public bool ShowEvent(int x)
	{
		if (x < EventList.Length) 
		{
			EventList[x].Show();
			return true;
		}
		else return false;
	}

	/** Displays event of name and returns the event. If there is no event of that name, it return false */
	public bool ShowEvent(string name)
	{
		for (int i=0; i<EventList.Length; i++)
		{
			if (name.Equals(EventList[i].Name))
			{
				EventList[i].Show();
				return true;
			}
		}
		return false;
	}
}
