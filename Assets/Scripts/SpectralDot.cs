using UnityEngine;
using System.Collections;

public class SpectralDot : MonoBehaviour 
{

	public GameObject dotSprite = null;


	public float dotAngle = 0.0f;

	//private float scaleState = 0;


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


	private Vector3 _originalScale; 
	public void SetOriginalScale (float s) 
	{
		_originalScale = new Vector3 (s, s, s);
	}

	private Vector3 _targetScale; 
	public void SetTargetScale (float s) 
	{
		_targetScale = new Vector3 (s, s, s);

		transform.localScale = _targetScale;

		//scaleState = 1;
	}

	public void SetDotColor (Color color) 
	{
		if (dotSprite != null) {
			dotSprite.GetComponent<Renderer> ().material.color = color;
		}
	}

	public void SetDotAngle (float startAngle) 
	{
		dotAngle += startAngle;
		transform.RotateAround (_centerPoint, Vector3.back, dotAngle);

	}

	public void SetDotScale (float scale) 
	{
		transform.localScale = new Vector3 (scale, scale, scale);

	}


	void Start () 
	{
		SetOriginalScale (0.025f);
		SetDotScale (0.025f);
		//SetTargetScale (0.1f);
	}
	
	void Update () 
	{
		
		transform.localScale = Vector3.Lerp (transform.localScale, _originalScale, 0.1f * Time.deltaTime);

	}
}
