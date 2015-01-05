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

	/** Adds score to list of high scores and saves it to memory, will cause a slow down in gameplay. Returns the location to which the score was added. */
	public int AddScore(int score)
	{
		int i = 0;
		bool added = false;
		int p = 5;
		while (i < numScores) 
		{
			if (scores[i] == null) {
				scores[i] = score;
				SaveScores ();
				p = 0;
			}
			else if (scores[i] >= score) {
				i++;
			}
			else {
				int temp = scores[i];
				scores[i] = score;
				score = temp;

				if(!added)
				{
					p = i;
					added = true;
				}
				i++;
			}
		}

		SaveScores ();
		return p;
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
