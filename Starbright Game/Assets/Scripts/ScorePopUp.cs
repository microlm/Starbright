using UnityEngine;
using System.Collections;

public class ScorePopUp : MonoBehaviour {

	public AnimationCurve opacity;
	public AnimationCurve size;

	int initSize;
	float timer;

	// Use this for initialization
	void Start () {
		timer = 0f;
		initSize = guiText.fontSize;
	}
	
	// Update is called once per frame
	void Update () {
		guiText.color = new Color (guiText.color.r, guiText.color.g, guiText.color.b, opacity.Evaluate (timer));
		guiText.fontSize = Mathf.FloorToInt(size.Evaluate (timer) * initSize);

		timer += Time.deltaTime;
	}
}
