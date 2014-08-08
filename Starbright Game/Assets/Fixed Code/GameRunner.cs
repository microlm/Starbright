using UnityEngine;
using System.Collections;

public class GameRunner {
	
	private static GameRunner game;
	private static GameState state;

	/* Static Gameplay Things */
	private int score;
	private Player mainCharacter;

	public GameRunner() {}

	public static GameRunner Game {
		get { 
			if (game == null )
				game = new GameRunner();
			return game;
		}
	}

	public GameState State {
		get { return state; }
		set {
			//only perfrom actions when going to a new state
			if (State != value )
			{
				GameState lastState = state;
				state = value;
				
				//do the stuff for the new state
				switch(value) {
				case (GameState.Start):
					Debug.Log("Starting...");
					//load first level
					Score = 0;
					//set main character
					State = GameState.Playing;
					break;
				case (GameState.MainMenu):
					//go to main menu
					break;
				case (GameState.Pause):
					Time.timeScale = 0.0f; //pause
					//bring up pause menu (still in current level)
					break;
				case (GameState.Playing):
					Debug.Log("Playing");
					if (lastState == GameState.Pause)
						Time.timeScale = 1.0f; //resume
					break;
				case (GameState.GameOver):
					Debug.Log("Game Over");
					//death. Drop back a level?
					break;
				case (GameState.Quit):
					Debug.Log("Quit");
					Application.Quit();
					break;
				}
			}
		}
	}

	public int Score {
		get { return score; }
		set { score = value; }
	}

	public Player MainCharacter {
		get { return mainCharacter; }
		set { 
			mainCharacter = value;
			Debug.Log("Setting main character...");
		}
	}
	
	public enum GameState {
		Default,
		Start,
		MainMenu,
		Playing,
		Pause,
		GameOver,
		Quit
	}
	
}