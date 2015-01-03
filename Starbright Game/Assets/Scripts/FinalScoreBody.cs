using UnityEngine;
using System.Collections;

public class FinalScoreBody : MonoBehaviour 
{
	
	public float mass;

	private Vector3 maxScale;
	private Vector3 cMaxScale;
	
	public float Mass 
	{
		get { return mass; }
		set { mass = value; }
	}
	
	public Vector3 Position 
	{
		get { return gameObject.transform.position; }
		set {
			//set value and lock z
			float z = gameObject.transform.position.z;
			gameObject.transform.position = new Vector3(value.x, value.y, z);
		}
	}

	public Vector3 Scale
	{
		get { return gameObject.transform.localScale; }
		private set { gameObject.transform.localScale = value; }
	}
	
	/** The Glow gameObject that is attached to Body*/
	public Glow GlowChild
	{
		get { return gameObject.GetComponentsInChildren<Glow>()[0]; }
	}
	
	public Color BodyColor
	{
		get { return GetComponent<SpriteRenderer> ().color; }
		set { GetComponent<SpriteRenderer> ().color = value; }
	}
	
	void Start ()
	{
		maxScale = Scale;
		cMaxScale = gameObject.collider2D.transform.localScale;
		BodyColor = ColorOption.Instance.assignColor(Mass);
	}
	
	void Update () 
	{
		Scale = ScaleByMass ();


	}
	
	
	Vector3 ScaleByMass()
	{
		return maxScale / 20 * Mass;
	}

}
