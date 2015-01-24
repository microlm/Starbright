using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialController : MonoBehaviour {

	public bool showTutorial;
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

	private bool IsPlaying
	{
		get
		{
			if (timer > 0)
				return true;
			else return false;
		}
	}

	/** Displays string text in tutorial overlay for time specified */
	public void DisplayText(string text, float time)
	{
		if (!IsPlaying)
			Text = text;
		else Text += '\n' + text;
		eventTime = time;
		timer = time;
	}

	/** Displays event x. If there is no event x or x has already played, it returns false */
	public bool ShowEvent(int x)
	{
		if (showTutorial)
		{
			if (x < EventList.Length) 
			{
				bool hasShown = EventList[x].HasShown;
				EventList[x].Show();
				return !hasShown;
			}
		}
		return false;
	}

	/** Displays event of name. If there is no event x or x has already played, it returns false */
	public bool ShowEvent(string name)
	{
		if (showTutorial)
		{
			for (int i=0; i<EventList.Length; i++)
			{
				if (name.Equals(EventList[i].Name))
				{
					bool hasShown = EventList[i].HasShown;
					EventList[i].Show();
					return !hasShown;
				}
			}
		}
		return false;
	}
}
