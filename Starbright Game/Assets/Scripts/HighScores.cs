using UnityEngine;
using System.Collections;

public class HighScores {

	private int[] scores;
	private int numScores = 5;

	public HighScores() 
	{
		scores = new int[numScores];
		LoadScores ();
	}

	/** Array of player scores from high to low */
	public int[] Scores
	{
		get { return scores; }
	}

	/** Adds score to list of high scores and saves it to memory, will cause a slow down in gameplay */
	public bool AddScore(int score)
	{
		int i = 0;
		bool added = false;
		while (i < numScores) 
		{
			if (scores[i] == null) {
				scores[i] = score;
				SaveScores ();
				return true;
			}
			else if (scores[i] >= score) {
				i++;
			}
			else {
				int temp = scores[i];
				scores[i] = score;
				score = temp;
				i++;
				added = true;
			}
		}

		SaveScores ();
		return added;
	}

	/** Loads player scores from memory */
	private void LoadScores()
	{
		for (int i=0; i < numScores; i++)
		{
			if (PlayerPrefs.HasKey("score" + i))
				scores[i] = PlayerPrefs.GetInt("score" + i);
		}
	}

	/** Saves current player scores to memory */
	private void SaveScores()
	{
		for (int i=0; i < numScores; i++)
		{
			if (scores[i] != null)
				PlayerPrefs.SetInt("score" + i, scores[i]);
		}

		PlayerPrefs.Save ();
	}

	/** Removes all of the player's high scores from memory */
	public void ClearScores()
	{
		PlayerPrefs.DeleteAll ();
		scores = new int[numScores];
	}
}
