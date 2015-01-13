using UnityEngine;
using System.Collections;

public class ColorOption : MonoBehaviour {

	public Gradient colorGradient;
	public float minMass = 5f;
	public float maxMass = 40f;

	public float luminance;
	public float saturation;
	public int colorCount;
	public int minDifference;
	public int range;

	private static ColorOption instance;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		instance = this;
	}

	void Start() {
		GenerateNewGradiant ();
	}

	public static ColorOption Instance {
		get { return instance; }
	}

	public Color assignColor(float mass) {
		float a = maxMass - minMass;
		float b = mass - minMass;
		return colorGradient.Evaluate(b/a);
	}

	public Color getColor(float percent) {
		return colorGradient.Evaluate(percent);
	}

	/** where h is an angle (degrees), 0 <= s <= 1, and 0 <= l <= 1 */
	public Color ColorFromHSL(int h, float s, float l)
	{
		h %= 360;
		float C = (1 - Mathf.Abs (2 * l - 1)) * s;
		float X = C * (1 - Mathf.Abs ((h / 60) % 2 - 1));
		float m = l - C / 2;
		if (h < 60) 
		{
			return new Color (C + m, X + m, 0 + m);
		}
		else if (h < 120) 
		{
			return new Color (X + m, C + m, 0 + m);
		}
		else if (h < 180) 
		{
			return new Color (0 + m, C + m, X + m);
		}
		else if (h < 240) 
		{
			return new Color (0 + m, X + m, C + m);
		}
		else if (h < 300) 
		{
			return new Color (X + m, 0 + m, C + m);
		}
		else  
		{
			return new Color (C + m, 0 + m, X + m);
		}
	}

	public void GenerateNewGradiant() {
		Color color = new Color ();
		GradientColorKey[] keys = new GradientColorKey[colorCount];
		int h = Random.Range (0, 360);
		for (int i=0; i < colorCount; i++)
		{
			keys[i] = new GradientColorKey(ColorFromHSL(h, saturation, luminance), (float) i/(colorCount-1));
			h += Random.Range(minDifference, minDifference + range);
		}
		Gradient gradient = new Gradient ();
		gradient.colorKeys = keys;
		colorGradient = gradient;
	}
}
