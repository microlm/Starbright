using UnityEngine;
using System.Collections;

public class FlashBehavior : MonoBehaviour {

	private bool flashed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGui()
	{
		Debug.Log (flashed);
		if (flashed)
		{
			Debug.Log ("FLASSSSH");
			transform.position = GameObject.Find ("Main Camera").transform.position;
			Debug.Log (transform.position);
			Texture2D flash = new Texture2D(1, 1);
			flash.SetPixel (1, 1, Color.white);
			flash.Apply ();
			GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), flash);
			StartCoroutine(SetFlashFalse());
		}
	}

	IEnumerator SetFlashFalse()
	{
		yield return new WaitForSeconds(1);
		flashed = false;
	}

	public void setFlash(bool f)
	{
		Debug.Log ("SET FLASH");
		flashed = f;
	}
}
