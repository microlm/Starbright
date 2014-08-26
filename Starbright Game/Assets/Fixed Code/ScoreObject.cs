using System;
using System.Collections.Generic;

public class ScoreObject {

	private int score;
	private int multiplier;

	public ScoreObject () {
		score = 0;
		multiplier = 1;
	}

	public int Value {
		get { return score; }
	}

	public int Multiplier {
		get { return multiplier; }
	}

	public void Incriment (int amt) {
		score += amt * multiplier;
	}

	public void IncreaseMultiplier () {
		multiplier++;
	}

	public void ResetMultiplier () {
		multiplier = 1;
	}

}

