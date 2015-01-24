using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialEvent
{
	[SerializeField]
	private string name;

	[SerializeField]
	private string text;

	[SerializeField]
	private float time;

	private bool hasShown;

	public TutorialEvent ()
	{
		hasShown = false;
	}

	public string Name 
	{
		get { return name; }
	}

	public bool HasShown
	{
		get { return hasShown; }
	}

	public void Show ()
	{
		if (!hasShown)
		{
			TutorialController.Instance.DisplayText (text, time);
			hasShown = true;
		}
	}
}
