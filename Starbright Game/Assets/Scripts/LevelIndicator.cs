using UnityEngine;
using System.Collections;

public class LevelIndicator : MonoBehaviour {

	public float rotationSpeed;
	public int nodesPerCircle = 10;
	public GameObject InactiveNode;
	public GameObject ActiveNode;

	public bool[] Active { get; private set; }
	public GameObject[] Nodes { get; private set; }

	private float rotation;
	private int degreesBetween;

	// Use this for initialization
	void Start () {
		Nodes = new GameObject[nodesPerCircle];
		Active = new bool[nodesPerCircle];
		degreesBetween = 360 / nodesPerCircle;
		for (int i=0; i<nodesPerCircle; i++)
		{
			SetNode(i, false);
		}
		rotation = 0f;

		UpdateLevel(1, true);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = PlayerCharacter.instance.Position;
		for (int i=0; i<nodesPerCircle; i++)
		{
			Nodes[i].transform.RotateAround (transform.position, Vector3.forward, rotationSpeed);
		}
		rotation += rotationSpeed;
	}

	public void UpdateLevel(int level, bool active)
	{
		Destroy(Nodes[level]);
		SetNode(level, active);
	}

	private void SetNode(int level, bool active)
	{
		Active[level] = active;
		if (active)
			Nodes[level] = (GameObject)GameObject.Instantiate(ActiveNode);
		else
			Nodes[level] = (GameObject)GameObject.Instantiate(InactiveNode);
		Nodes[level].transform.parent = transform;
		Nodes[level].transform.RotateAround (transform.position, Vector3.forward, rotation + (degreesBetween * level));
	}
}
