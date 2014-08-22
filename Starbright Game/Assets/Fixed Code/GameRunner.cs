using UnityEngine;
using System.Collections;

public class GameRunner {
	
	private static GameRunner game;
	private static GameState state;

	/* Static Gameplay Things */
	private ScoreObject score;
	private Player mainCharacter;
	private ColorOption colors;

	private const string LEVEL_ONE = "Asteroids";
	private const string PLAYER = "PC";
	private const string COLORS = "ColorOptions";

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

					Debug.Log("Loading Asteroids Level...");
					Application.LoadLevel(LEVEL_ONE);

					//assign values to everything
					score = new ScoreObject();
					MainCharacter = GameObject.Find(PLAYER).GetComponent<Player>();
					colors = GameObject.Find(COLORS).GetComponent<ColorOption>();

					State = GameState.Playing;
					break;
				case (GameState.MainMenu):
					//go to main menu
					break;
				case (GameState.Pause):
					Debug.Log("Pause");
					Time.timeScale = 0.0f; //pause
					//bring up pause menu (still in current level)
					break;
				case (GameState.Playing):
					Debug.Log("Playing...");
					if (lastState == GameState.Pause)
						Time.timeScale = 1.0f; //resume
					break;
				case (GameState.Death):
					Debug.Log ("Death State");
					//death. Drop back a level?
					break;
				case (GameState.GameOver):
					Debug.Log("Game Over");
					//game over screen
					break;
				case (GameState.Quit):
					Debug.Log("Quit");
					Application.Quit();
					break;
				default:
					Debug.LogError("Invalid Game State");
					Debug.Break();
					break;
				}
			}
		}
	}

	public ScoreObject Score {
		get { return score; }
	}

	public Player MainCharacter {
		get { return mainCharacter; }
		set { 
			mainCharacter = value;
			Debug.Log("Setting main character...");
			if (value == null)
				Debug.LogError("Could not find and set main character");
		}
	}

	public ColorOption Colors {
		get { return colors; }
	}
	
	public enum GameState {
		Default,
		Start,
		MainMenu,
		Playing,
		Pause,
		Death,
		GameOver,
		Quit
	}
	
}