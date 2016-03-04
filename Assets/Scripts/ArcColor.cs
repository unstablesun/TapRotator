using UnityEngine;
using System.Collections;

public class ArcColor : MonoBehaviour {

	//task

	private float _elaspedTime = 0.0f;
	private float _taskTime = 0.0f;

	// Use this for initialization
	void Start () 
	{
		



	}
	
	void Update () 
	{



		_elaspedTime += Time.deltaTime;
		if (_elaspedTime > _taskTime) {


			//determine next task

			int rInt = UnityEngine.Random.Range (0, 100);

			float fInt = UnityEngine.Random.value;





			_elaspedTime = 0.0f;			
		}


	}
}
