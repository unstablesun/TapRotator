using UnityEngine;
using System.Collections;

public class AudioBeatDetector : MonoBehaviour 
{

	private AudioSource aSource;

	private float[] samplesL = new float[256];
	private float[] samplesR = new float[256];

	private float[] spectrum = new float[64];

	private float threshold = 0.5f;
	private float lastPeak = 0f;
	private float fallOff = 0.98f;

	void Awake () 
	{
		//Get and store a reference to the following attached components: 
		//AudioSource
		this.aSource = GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start () 
	{
		aSource.Play ();

	}
	
	// Update is called once per frame
	void Update () 
	{
		GetDynamicSpectrum();

		GetDynamicOutput ();
	}


	private void GetDynamicSpectrum()
	{
		aSource.GetSpectrumData(this.spectrum, 0, FFTWindow.BlackmanHarris);
	}
		


	private void GetDynamicOutput()
	{
		//Channel 1
		aSource.GetOutputData (this.samplesL, 0);

		float average = 0.0f;
		float max = 0.0f;
		//float min = 10000.0f;
		for(int i=0; i<samplesL.Length;i++)
		{
			if (samplesL [i] < 0) {

				float v = samplesL [i] * -1.0f;
				average += v;

				if(v > max)
					max = v;

			} else {

				float v = samplesL [i];
				average += samplesL [i];

				if(v > max)
					max = v;

			}
		}
		average /= samplesL.Length;

		//DrawSingleCube(0, average);
		//DrawSingleCube(1, max);


		//Channel 2
		aSource.GetOutputData (this.samplesL, 1);

		average = 0.0f;
		max = 0.0f;
		//min = 10000.0f;
		for(int i=0; i<samplesL.Length;i++)
		{
			if (samplesL [i] < 0) {

				float v = samplesL [i] * -1.0f;
				average += v;

				if(v > max)
					max = v;

			} else {

				float v = samplesL [i];
				average += samplesL [i];

				if(v > max)
					max = v;

			}


		}
		//Debug.Log ("averageRaw = " + average.ToString());
		average /= samplesL.Length;

		if (max > threshold) {
			lastPeak = max;

			Debug.Log ("average = " + average.ToString() + " max = " + max.ToString());

		}

		lastPeak *= fallOff;
		//DrawSingleCube(3, average);
		//DrawSingleCube(2, max);



	}

}
