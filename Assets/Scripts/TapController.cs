using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TapController : MonoBehaviour {


	private List <GameObject> TapObjects = null;

	public GameObject CenterPoint;
	public GameObject Zenith;
	public Animation ZAnim;

	private float _nadir = 180.0f;

	private float _elaspedTime = 0.0f;
	private float _insertTapTime = 0.5f;

	private float _riseAngle = 350.0f;
	private float _fallAngle = 10.0f;


	private float _noteSpacing = 30.0f;

	//private float _lastTime = 0.0f;
	//private float _detaPressTime = 0.0f;
	private float _degressPerSecond = 0.0f;

	//float averageDeltaTime = 0f;
	//float lastBeatTime = 0f;


	//private int _byeIn = 0;
	private bool _insertTapIndicator = false;

	AudioBeatDetector audioBeatDetectorScript = null;




	void Awake () 
	{
		TapObjects = new List<GameObject>();

	}

	void Start () 
	{
		for (int t = 0; t < 64; t++) 
		{
			GameObject _tapObj = Instantiate (Resources.Load ("Prefabs/TapObject", typeof(GameObject))) as GameObject;

			_tapObj.transform.position = new Vector3(Zenith.transform.position.x, Zenith.transform.position.y * -1.0f, Zenith.transform.position.z);


			TapObject tapObjectScript = _tapObj.GetComponent<TapObject> ();

			tapObjectScript.SetCenterPoint (new Vector3(CenterPoint.transform.position.x, CenterPoint.transform.position.y, CenterPoint.transform.position.z));
			tapObjectScript.SetZenithPoint (new Vector3(Zenith.transform.position.x, Zenith.transform.position.y, Zenith.transform.position.z));

			TapObjects.Add (_tapObj);
		}

		FindBeatDetector ();

		audioBeatDetectorScript.StartSampling (4);
	}
	
	void Update () {


		//poll for real beat


		DetermineBeat ();



		if (Input.GetMouseButtonDown (0)) {

			// check if any tap objects are with tolerance of apex
			float currentTime = Time.time;

			/*
			if (_lastTime > 0.0) {
				_detaPressTime = currentTime - _lastTime;
				_byeIn++;

				if (_byeIn > 2 && _insertTapIndicator == false) {

					_insertTapTime = _detaPressTime;
					_degressPerSecond = _noteSpacing / _detaPressTime;

					_insertTapIndicator = true;

					AddBeatIndicator (180.0f - _noteSpacing, _degressPerSecond);
					AddBeatIndicator (180.0f - _noteSpacing * 2, _degressPerSecond);
					AddBeatIndicator (180.0f - _noteSpacing * 3, _degressPerSecond);
				}
			}
			*/

			//_lastTime = Time.time;

			GameObject _tObj = QueryTapObjectsOnTap ();

			if (_tObj == null) {
				//record miss
				//if (_insertTapIndicator == true && _byeIn > 3) {
				//	_insertTapIndicator = false;
				//	_byeIn = 0;
					//clear all tap indicators?

					//QueryTapObjectReset ();
				//}
			} else {
				//successful 
				TapObject tapObjectScript = _tObj.GetComponent<TapObject> ();

				tapObjectScript.SetTapColor (new Color (0.1f, 0.88f, 0.44f, 1.0f));

				ZAnim.Play ();
			}
		}
			
		QueryTapObjectsUpSlope ();

		_elaspedTime += Time.deltaTime;
		if (_elaspedTime > _insertTapTime) {

			if (_insertTapIndicator == true) {
				_elaspedTime = 0.0f;

				AddBeatIndicator (_nadir - _noteSpacing * 3, _degressPerSecond);
			}
		}
	
		QueryTapObjectCreate ();


		if (QueryTapObjectsDebug ()) {
			Debug.Log ("Error tObj Conflict");
		}

	}


	private void AddBeatIndicator(float angle, float velocity)
	{
		GameObject _tObj = GetAvailableTapObject ();
		if (_tObj != null) {
			TapObject tObjectScript = _tObj.GetComponent<TapObject> ();
			if (tObjectScript != null) {
				tObjectScript._state = TapObject.eState.UpSlope;
				tObjectScript.SetPositionToNadir ();
				tObjectScript.SetStartAngle (angle);
				tObjectScript.SetStartVelocity (velocity);
			} else {
				Debug.Log("tObjectScript = null : no script attached");
			}
		} else {
			Debug.Log("tObj = null : error object not found");
		}
	}

	void QueryTapObjectsUpSlope() 
	{
		foreach(GameObject tObj in TapObjects)
		{
			TapObject tapObjectScript = tObj.GetComponent<TapObject> ();
			if(tapObjectScript._state == TapObject.eState.UpSlope)
			{
				float cAngle = tapObjectScript.currentAngle;

				//check tolerance
				if (cAngle < 170.0f) {
					tapObjectScript._state = TapObject.eState.DownSlope;
				}
			}
		}
	}

	GameObject QueryTapObjectsOnTap() 
	{
		foreach(GameObject tObj in TapObjects)
		{
			TapObject tapObjectScript = tObj.GetComponent<TapObject> ();
			if(tapObjectScript._state == TapObject.eState.UpSlope || tapObjectScript._state == TapObject.eState.DownSlope)
			{
				float cAngle = tapObjectScript.currentAngle;

				//check tolerance
				if (cAngle > _riseAngle || cAngle < _fallAngle) {
					return tObj;
				}
			}
		}
		return null;
	}

	GameObject GetAvailableTapObject() 
	{
		foreach(GameObject tObj in TapObjects)
		{
			TapObject tapObjectScript = tObj.GetComponent<TapObject> ();
			if(tapObjectScript._state == TapObject.eState.InPool)
			{
				return tObj;

			}
		}
		return null;
	}
		
	void QueryTapObjectCreate() 
	{
		foreach(GameObject tObj in TapObjects)
		{
			TapObject tapObjectScript = tObj.GetComponent<TapObject> ();
			if(tapObjectScript._state == TapObject.eState.Reset)
			{
				tapObjectScript._state = TapObject.eState.InPool;
			}
		}
	}

	void QueryTapObjectReset() 
	{
		foreach(GameObject tObj in TapObjects)
		{
			TapObject tapObjectScript = tObj.GetComponent<TapObject> ();
			tapObjectScript.ResetObj ();
			if(tapObjectScript._state == TapObject.eState.Reset)
			{
				tapObjectScript._state = TapObject.eState.InPool;
			}
		}
	}
		
	bool QueryTapObjectsDebug() 
	{
		bool noConflict = false;
		int numInRange = 0;
		foreach(GameObject tObj in TapObjects)
		{
			TapObject tapObjectScript = tObj.GetComponent<TapObject> ();
			if(tapObjectScript._state == TapObject.eState.UpSlope || tapObjectScript._state == TapObject.eState.DownSlope)
			{
				float cAngle = tapObjectScript.currentAngle;

				//check tolerance
				if (cAngle > _riseAngle || cAngle < _fallAngle) {
					numInRange++;
				}
			}
		}
		if (numInRange > 1)
			noConflict = true;

		return noConflict;
	}


	private void DetermineBeat()
	{
		if (audioBeatDetectorScript.areBeatDeltasReady() == true) {
		
			float averageDeltaTime = audioBeatDetectorScript.getBeatsAverageDeltaTime ();
			//float lastBeatTime = audioBeatDetectorScript.getLastBeatTime ();

			_insertTapTime = averageDeltaTime;
			_degressPerSecond = _noteSpacing / averageDeltaTime;


			if (_insertTapIndicator == false) {
				_insertTapIndicator = true;

				//add initial beats
				AddBeatIndicator (_nadir - _noteSpacing, _degressPerSecond);
				AddBeatIndicator (_nadir - _noteSpacing * 2, _degressPerSecond);
				AddBeatIndicator (_nadir - _noteSpacing * 3, _degressPerSecond);

				_elaspedTime = 0;//probally needs to reset somewhere, here?
			}

			Debug.Log ("DetermineBeat - successFulStart : averageDeltaTime = " + averageDeltaTime);
		}

		//if(audioBeatDetectorScript.isSampleReady() == true) {
		//	audioBeatDetectorScript.StartSampling (4);
		//}
		
	}

	private void FindBeatDetector()
	{
		GameObject _audioBeatDetectorObject = GameObject.Find ("AudioBeatDetector");
		if (_audioBeatDetectorObject != null) {
			audioBeatDetectorScript = _audioBeatDetectorObject.GetComponent<AudioBeatDetector> ();
			if(audioBeatDetectorScript != null) {
				return;
			}
			Debug.Log ("_audioBeatDetectorScript = null");
		}
		Debug.Log ("_audioBeatDetectorObject = null");
	}

		
}
