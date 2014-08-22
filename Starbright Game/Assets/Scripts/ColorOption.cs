using UnityEngine;
using System.Collections;

public class ColorOption : MonoBehaviour {

	public Gradient colorGradient;
	public float maxMass = 40f;

	public Color assignColor(float mass) {
		return colorGradient.Evaluate(mass/maxMass);
	}

	public Color getColor(float percent) {
		return colorGradient.Evaluate
	}
}
