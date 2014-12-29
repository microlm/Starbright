using UnityEngine;
using System.Collections;

public class ScoreCameraBehavior : MonoBehaviour {

	public ParticleSystem explosion;
	public AnimationCurve snZoomSpeed;
	public AnimationCurve pcZoomSpeed;
	public AnimationCurve pcCameraSpeed;

	float duration;
	float speed;
	Vector3 lastPlayerPosition;
	GameObject player;

	public bool CameraShake; //turn on/off camera shake
	public float MaxShakeTime;	//Max shake time
	public float MaxShakeAmount; //max shake movement
	private float shakeTime; // shakes if shake time >0
	private float shakeAmount;
	protected float cameraDepth;

	// Use this for initialization
	void Start () 
	{
		duration = 0f;
		player = PlayerCharacter.instance.gameObject;
		lastPlayerPosition = player.transform.position;
		cameraDepth = camera.transform.position.z;
		MaxShakeTime = 3f;
		shakeTime = MaxShakeTime;

		// sets the size of the camera using base settings where if the player character was 15f in size, then the camera's size was 4f.
		float currentMass = player.GetComponent<FinalScorePlayerBehavior>().getMass ();
		camera.orthographicSize = (4f/15f) * currentMass;
		shakeAmount = currentMass * 2f;
		MaxShakeAmount = shakeAmount/5f;

		float adjustment = player.transform.position.x + (camera.orthographicSize * Screen.width/Screen.height * 1.1f);
		Debug.Log (adjustment);
		camera.transform.position = new Vector3(adjustment, camera.transform.position.y, camera.transform.position.z);

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (shakeTime < MaxShakeTime)
		{
			ShakeCameraMovement();
			shakeTime += Time.deltaTime;
		}
		//if shaking has just ended
		else if (shakeTime > MaxShakeTime) 
		{
			shakeTime = MaxShakeTime; 
		}
	}

	public void PlayerCameraBehavior()
	{
		duration += Time.deltaTime;
		speed = duration/5f;

		camera.transform.position = camera.transform.position + ((player.transform.position - lastPlayerPosition) * pcCameraSpeed.Evaluate(speed));
	
	
		lastPlayerPosition = player.transform.position;
	}

	public void SuperNovaZoom()
	{
		duration += Time.deltaTime;
		speed = duration/explosion.duration;
		if(explosion.duration > duration)
		{
			camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, camera.orthographicSize * snZoomSpeed.Evaluate(speed), Time.deltaTime);
		}
	}

	public void resetDuration()
	{
		duration = 0f;
	}

	public void Shake(float percentStrength = 1f)
	{
		shakeTime = 0f;
		shakeAmount = MaxShakeAmount * percentStrength;
	}

	public void ShakeCameraMovement()
	{
		if (shakeAmount > 0) 
		{
			//decrease shake amount over time
			shakeAmount -= MaxShakeAmount * (shakeTime / MaxShakeTime);
		}
		
		//random position 
		float x = camera.transform.position.x + PositiveOrNegative() * shakeAmount * UnityEngine.Random.Range(0f, 1f);
		float y = camera.transform.position.y + PositiveOrNegative() * shakeAmount * UnityEngine.Random.Range(0f, 1f);

		//Debug.Log (shakeAmount + " " + x + " " + y);
		
		Vector3 shakePosition = new Vector3 (x, y, transform.position.z);
		camera.transform.position = Vector3.Lerp(transform.position, shakePosition, Time.deltaTime);
		camera.transform.position = new Vector3(transform.position.x, transform.position.y, cameraDepth);
	}

	private int PositiveOrNegative()
	{
		//there is probably a better way to do this but 
		//I just want it dine quick
		
		int temp = UnityEngine.Random.Range(0, 2);
		if (temp == 0)
			return -1;
		return 1;
	}
}
