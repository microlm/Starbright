using UnityEngine;
using System.Collections;

public class ColorOption : MonoBehaviour {

	public Gradient colorGradient;
	public float minMass = 5f;
	public float maxMass = 40f;

	public int colorCount;
	public float luminance;
	public float saturation;

	[Header ("Color Harmony Options")]
	public float offsetAngle1;
	public float offsetAngle2;
	public float rangeAngle0;
	public float rangeAngle1;
	public float rangeAngle2;

	private float goldenRatio = 0.618033988749895f;

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
	public Color ColorFromHSL(float h, float s, float l)
	{
		h %= 360f;
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

		float referenceAngle = Random.Range(0f, 360f);
		for (int i=0; i < colorCount; i++)
		{
			float h = Random.Range(0f, 1f) * (rangeAngle0 + rangeAngle1 + rangeAngle2);

			if (h > rangeAngle0)
			{
				if (h < rangeAngle0 + rangeAngle1)
				{
					h += offsetAngle1;
				}
				else 
				{
					h += offsetAngle2;
				}
			}
			keys[i] = new GradientColorKey(
				ColorFromHSL(
				(h + referenceAngle),
				saturation, luminance), 
				(float) i/(colorCount-1));
		}
		Gradient gradient = new Gradient ();
		gradient.colorKeys = keys;
		colorGradient = gradient;
	}
}
