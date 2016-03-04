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


	private bool firstBeat = true;
	private float lastBeatTime = 0.0f;
	private float beatNumDeltaSamples = 0.0f;
	private float beatsDeltaTimeTotal = 0.0f;
	private float beatsAverageDeltaTime = 0.0f;
	private bool beatDeltasReady = false;

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

	public bool areBeatDeltasReady()
	{
		return beatDeltasReady;
	}

	public float getBeatsAverageDeltaTime()
	{
		return beatsAverageDeltaTime / beatNumDeltaSamples;
	}


	private void GetDynamicSpectrum()
	{
		aSource.GetSpectrumData(this.spectrum, 0, FFTWindow.BlackmanHarris);
	}
		


	private void GetDynamicOutput()
	{
		
		//Channel 1
		aSource.GetOutputData (this.samplesL, 0);

		float averageL = 0.0f;
		float maxL = 0.0f;
		//float min = 10000.0f;
		for(int i=0; i<samplesL.Length;i++)
		{
			if (samplesL [i] < 0) {

				float v = samplesL [i] * -1.0f;
				averageL += v;

				if(v > maxL)
					maxL = v;

			} else {

				float v = samplesL [i];
				averageL += samplesL [i];

				if(v > maxL)
					maxL = v;
			}
		}
		averageL /= samplesL.Length;

		//Channel 2
		aSource.GetOutputData (this.samplesR, 1);

		float averageR = 0.0f;
		float maxR = 0.0f;
		//min = 10000.0f;
		for(int i=0; i<samplesL.Length;i++)
		{
			if (samplesL [i] < 0) {

				float v = samplesL [i] * -1.0f;
				averageR += v;

				if(v > maxR)
					maxR = v;

			} else {

				float v = samplesL [i];
				averageR += samplesL [i];

				if(v > maxR)
					maxR = v;
			}
		}
		averageR /= samplesL.Length;


		float averageMax = (maxL + maxR) / 2.0f;

		if (lastPeak < threshold && averageMax > threshold) {
			lastPeak = averageMax;

			if (firstBeat == true) {
			
				lastBeatTime = Time.time;
			} else {

				float beatTime = Time.time;
				float deltaTime = beatTime - lastBeatTime;

				if (beatNumDeltaSamples < 32) {

					//beatDeltas [beatDeltaIndex] = deltaTime;

					beatsDeltaTimeTotal += deltaTime;
					beatNumDeltaSamples++;

					beatDeltasReady = true;
				}
			}
				
			Debug.Log ("average = " + averageMax.ToString() + " maxL = " + maxL.ToString() + " maxR = " + maxR.ToString());
		}

		lastPeak *= fallOff;

	}

}
