using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour {

	public float swipeSpeed;

	private SpaceBody target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void TapSelect() {

		//keep track of how many touches at once
		float count = 0;
		
		foreach (Touch touch in Input.touches) {

			//if swyped
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
				
				// Get movement of the finger since last frame
				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
				
				// Move camera by swipe
				Camera.main.GetComponent<CameraBehavior>().Scroll(new Vector3(-touchDeltaPosition.x * swipeSpeed,
				                                                         -touchDeltaPosition.y * swipeSpeed, 0));
			}
			
			if (touch.phase == TouchPhase.Began) {
				Ray ray = Camera.main.ScreenPointToRay(touch.position);
				RaycastHit hit ;
				
				//if touch planet object
				if (Physics.Raycast (ray, out hit)) {

					//start orbiting
					target = hit.transform.GetComponent<SpaceBody>();
					GameRunner.Game.MainCharacter.StartOrbit(target);
			
				}
			}
			
			//when stop touching
			if(touch.phase == TouchPhase.Ended){
				//stop orbiting
				if(GameRunner.Game.MainCharacter.IsOrbiting) {
					GameRunner.Game.MainCharacter.StopOrbit();
				}

				//and stop scrolling
				Camera.main.GetComponent<CameraBehavior>().StopScroll();
			}
			
			count++;
			
		}
		
		
		//four fingers quits
		if (count >= 4)
			Application.Quit ();
		//three fingers restarts
		if (count >= 3)
			Application.LoadLevel (Application.loadedLevel);
		
		
	}
}
