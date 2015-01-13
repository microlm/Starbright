using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuLoadScore : MonoBehaviour {

	public int rank;
	
	void OnEnable()
	{
		HighScores hi = new HighScores();

		int i = rank - 1;

		if(hi.Scores[i] == 0)
		{
			gameObject.GetComponent<Text>().text = (i + 1) + "";
		}
		else
		{
			gameObject.GetComponent<Text>().text = (i + 1) + "        " + hi.Scores[i].ToString();
		}
		
	}
}
