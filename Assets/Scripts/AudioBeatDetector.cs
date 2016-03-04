using UnityEngine;
using System.Collections;

public class AudioBeatDetector : MonoBehaviour 
{

	public GameObject LeftChannelSprite;
	public GameObject RightChannelSprite;
	public GameObject BaseChannelSprite;
	public GameObject ThresholdLevelSprite;
	public GameObject MaxLevelSprite;

	private AudioSource aSource;

	private float[] samplesL = new float[256];
	private float[] samplesR = new float[256];

	private float[] spectrum = new float[64];

	private float threshold = 0.5f;
	private float lastPeak = 0f;
	private float fallOff = 0.98f;

	private float onSetTimeOffset = 0.0f;

	private bool processBeats = false;

	private bool firstBeat = true;
	private float lastBeatTime = 0.0f;
	private float numToSample = 4.0f;
	private float beatNumDeltaSamples = 0.0f;
	private float beatsDeltaTimeTotal = 0.0f;
	private bool beatDeltasReady = false;

	//channel stuff
	private Vector3 maxLevelPos;
	private Vector3 thresholdLevelPos;
	private Vector3 leftChannelPos;
	private Vector3 rightChannelPos;

	private Transform baseChannelTransform = null;
	private Transform thresholdLevelTransform = null;
	private Transform maxLevelTransform = null;

	private Transform leftChannelTransform = null;
	private Transform rightChannelTransform = null;

	private float meterMax = 5.0f;

	private Vector3 gravity = new Vector3(0.0f,0.1f,0.0f);

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

		baseChannelTransform = BaseChannelSprite.transform;
		thresholdLevelTransform = ThresholdLevelSprite.transform;
		maxLevelTransform = MaxLevelSprite.transform;
		leftChannelTransform = LeftChannelSprite.transform;
		rightChannelTransform = RightChannelSprite.transform;


		//set visual threshold level
		thresholdLevelPos.Set (thresholdLevelTransform.position.x, baseChannelTransform.position.y + (meterMax * threshold), thresholdLevelTransform.position.z);
		thresholdLevelTransform.position = thresholdLevelPos;

		//set visual max level
		maxLevelPos.Set (maxLevelTransform.position.x, baseChannelTransform.position.y + meterMax, maxLevelTransform.position.z);
		maxLevelTransform.position = maxLevelPos;

	}
	
	// Update is called once per frame
	void Update () 
	{
		GetDynamicSpectrum();

		GetDynamicOutput ();

		if (leftChannelTransform.position.y > baseChannelTransform.position.y) {
			leftChannelTransform.position -= gravity;
		} else {
			//leftChannelTransform.position.y = baseChannelTransform.position.y;
		}
			
		if (rightChannelTransform.position.y > baseChannelTransform.position.y) {
			rightChannelTransform.position -= gravity;
		} else {
			//rightChannelTransform.position.y = baseChannelTransform.position.y;
		}

	}


	public bool isSampleReady()
	{
		return (processBeats == false);
	}

	public void StartSampling(float num)
	{
		processBeats = true;
		firstBeat = true;
		beatDeltasReady = false;

		beatNumDeltaSamples = 0.0f;
		beatsDeltaTimeTotal = 0.0f;

		numToSample = num;
	}
		
	public bool areBeatDeltasReady()
	{
		bool ready = beatDeltasReady;
		beatDeltasReady = false;

		return ready;
	}

	public float getBeatsAverageDeltaTime()
	{
		float averageDeltaTime = beatsDeltaTimeTotal / beatNumDeltaSamples;

		//Debug.Log ("getBeatsAverageDeltaTime : averageDeltaTime = " + averageDeltaTime.ToString());

		return averageDeltaTime;
	}

	public float getLastBeatTime()
	{
		return lastBeatTime - onSetTimeOffset;
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


			leftChannelPos.Set(leftChannelTransform.position.x, baseChannelTransform.position.y + (maxL * meterMax), leftChannelTransform.position.z);
			rightChannelPos.Set(rightChannelTransform.position.x, baseChannelTransform.position.y + (maxR * meterMax), rightChannelTransform.position.z);

			leftChannelTransform.position = leftChannelPos;
			rightChannelTransform.position = rightChannelPos;


			lastPeak = averageMax;

			if (processBeats) {
				
				if (firstBeat == true) {
			
					lastBeatTime = Time.time;
					firstBeat = false;

				} else {

					float beatTime = Time.time;
					float deltaTime = beatTime - lastBeatTime;
					lastBeatTime = Time.time;

					if (beatNumDeltaSamples < numToSample) {

						beatsDeltaTimeTotal += deltaTime;
						beatNumDeltaSamples += 1.0f;
					} else {
						beatDeltasReady = true;	

						processBeats = false;
					}
				}
			}


			Debug.Log ("average = " + averageMax.ToString() + 
						" maxL = " + maxL.ToString() + 
						" maxR = " + maxR.ToString() + 
						" beatNumDeltaSamples = " + beatNumDeltaSamples.ToString() + 
						" beatsDeltaTimeTotal = " + beatsDeltaTimeTotal.ToString());
		}

		lastPeak *= fallOff;

	}

}
