using UnityEngine;
using System.Collections;

public class ArcColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		

		//distance =Vector3.Distance(transform.position,target.position);


		//Vector3 heading = target.position - player.position;


		RaycastHit hit;
		float distanceToGround = 0;

		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F)) {
			distanceToGround = hit.distance;
		}


	}
	
	// Update is called once per frame
	void Update () {
		//GetComponent<Renderer>().material.SetFloat("_Cutoff", Mathf.InverseLerp(0, Screen.width, Input.mousePosition.x)); 
	
	}
}
