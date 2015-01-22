using UnityEngine;
using System.Collections;

public class Game {
	private static Game instance;
	private GameState state;

	public Game ()
	{
		state = GameState.Playing;
	}

	public static Game Instance {
		get 
		{
			if (instance == null)
				instance = new Game();
			return instance;
		}
	}

	public GameState State
	{
		get { return state; }
	}

	public void Start(string level) 
	{
		state = GameState.Playing;
		Application.LoadLevel (level);
	}

	public void EndGame(string level)
	{
		if (State == GameState.Playing)
		{
			state = GameState.GameOver;
			Application.LoadLevel (level);
		}
	}

	public void GoToMenu(string menu)
	{
		if (State == GameState.Paused)
			Resume ();
		Application.LoadLevel (menu);
		state = GameState.Menu;
	}

	public void Pause() 
	{
		if (State == GameState.Playing)
		{
			state = GameState.Paused;
			Time.timeScale = 0f;
		}
	}

	public void Resume()
	{
		if (State == GameState.Paused)
		{
			state = GameState.Playing;
			Time.timeScale = 1f;
		}
	}

	public void Quit()
	{
		Application.Quit ();
		Application.runInBackground = false;
	}
}
