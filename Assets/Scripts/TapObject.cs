using UnityEngine;
using System.Collections;

public class TapObject : MonoBehaviour 
{
	//note: in this system the zenith is 0 degress and nadir is at 180 degress
	//degrees increament in a clockwise direction
	//tap objs are inserted into the system between 180 and 360
	//tap objs that are in a reset state are positioned at nadir
	//when setting the starting angle 

	public enum eState 
	{
		Reset,
		InPool, 
		UpSlope,  
		DownSlope, 
		FadeOut
	};
	public eState _state = eState.InPool;

	public GameObject tapSprite = null;

	private float velocity = 0.0f;
	public float currentAngle = 0.0f;
	private float _elaspedTime = 0.0f;

	private Vector3 _centerPoint; 
	public void SetCenterPoint (Vector3 c) 
	{
		_centerPoint = new Vector3 (c.x, c.y, c.z);
	}
		
	private Vector3 _zenithPoint; 
	public void SetZenithPoint (Vector3 c) 
	{
		_zenithPoint = new Vector3 (c.x, c.y, c.z);
	}

	public void SetPositionToNadir () 
	{
		transform.position = new Vector3 (_zenithPoint.x, _zenithPoint.y * -1.0f, _zenithPoint.z);
		currentAngle = 180.0f;
	}
		
	public void SetStartAngle (float startAngle) 
	{
		currentAngle += startAngle;
		transform.RotateAround (_centerPoint, Vector3.back, startAngle);

	}

	public void SetStartVelocity (float startVelocity) 
	{
		velocity = startVelocity;

	}

	public void SetTapColor (Color color) 
	{
		if (tapSprite != null) {
			tapSprite.GetComponent<Renderer> ().material.color = color;
		}
	}

	public void ResetObj () 
	{
		_state = eState.Reset;
		SetPositionToNadir();
		tapSprite.GetComponent<Renderer> ().material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	}



	void Start () 
	{
		_elaspedTime = 0.0f;
	}

	void Update ()
	{

		if (_state == eState.UpSlope || _state == eState.DownSlope || _state == eState.FadeOut) {

			float angleDelta = Time.deltaTime * velocity;
			currentAngle += angleDelta;

			if (currentAngle >= 360.0f) {
				currentAngle -= 360.0f;
			}

			transform.RotateAround (_centerPoint, Vector3.back, angleDelta);

			//Debug.Log ("angle = " + currentAngle);
		}


		if (_state == eState.DownSlope) {

			if (currentAngle > 90.0f) {
				_state = eState.FadeOut;
			}
		}

		if (_state == eState.FadeOut) {
			
			Color c = tapSprite.GetComponent<Renderer> ().material.color;
			float alpha = c.a;
			alpha *= 0.95f;

			tapSprite.GetComponent<Renderer> ().material.color = new Color (c.r, c.g, c.b, alpha);

			_elaspedTime += Time.deltaTime;
			if (_elaspedTime > 0.6f) {
				_elaspedTime = 0.0f;

				ResetObj ();
			}
		}
	}
			
}
