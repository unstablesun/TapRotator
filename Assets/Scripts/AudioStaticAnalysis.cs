using UnityEngine;
using System.Collections;

public class AudioStaticAnalysis : MonoBehaviour {

	//An AudioSource object so the music can be played
	private AudioSource aSource;
	//A float array that stores the audio samples
	public float[] samples = new float[32];
	//A renderer that will draw a line at the screen
	private LineRenderer lRenderer;
	//A reference to the cube prefab
	public GameObject cube;

	public float Hscale = 0.5f;
	public float fallOff = 0.1f;


	private float[] samplesL = new float[256];
	private float[] samplesR = new float[256];

	//The transform attached to this game object
	private Transform goTransform;
	//The position of the current cube. Will also be the position of each point of the line.
	private Vector3 cubePos;
	//An array that stores the Transforms of all instantiated cubes
	private Transform[] cubesTransform;
	//The velocity that the cubes will drop
	private Vector3 gravity = new Vector3(0.0f,1.5f,0.0f);

	void Awake () 
	{
		//Get and store a reference to the following attached components: 
		//AudioSource
		this.aSource = GetComponent<AudioSource>();
		//LineRenderer
		this.lRenderer = GetComponent<LineRenderer>();
		//Transform
		this.goTransform = GetComponent<Transform>();
	}

	void Start()
	{
		gravity = new Vector3(0.0f, fallOff, 0.0f);

		//The line should have the same number of points as the number of samples
		lRenderer.SetVertexCount(samples.Length);
		//The cubesTransform array should be initialized with the same length as the samples array
		cubesTransform = new Transform[samples.Length];
		//Center the audio visualization line at the X axis, according to the samples array length
		goTransform.position = new Vector3(-(4/2) * Hscale, goTransform.position.y, goTransform.position.z);

		//Create a temporary GameObject, that will serve as a reference to the most recent cloned cube
		GameObject tempCube;

		//For each sample
		for(int i=0; i<4;i++)
		{
			//Instantiate a cube placing it at the right side of the previous one
			tempCube = (GameObject) Instantiate(cube, new Vector3(goTransform.position.x + (i * Hscale), goTransform.position.y, goTransform.position.z),Quaternion.identity);
			//Get the recently instantiated cube Transform component
			cubesTransform[i] = tempCube.GetComponent<Transform>();
			//Make the cube a child of this game object
			cubesTransform[i].parent = goTransform;
		}


		//aSource.time = 10.0f;
		//aSource.GetSpectrumData(this.samples, 0, FFTWindow.BlackmanHarris);

		//float[] samples = aSource.GetOutputData (256, 0);
		//DrawStaticSpectrum ();


		//float playbackPos = aSource.time;

		//int playbackSamplesPos = aSource.timeSamples;
		aSource.Play ();


		//GetAudioSamples ();
	}

	void Update () 
	{

		//aSource.pitch = 0.5f;

		//aSource.GetSpectrumData(this.samples, 0, FFTWindow.BlackmanHarris);

		//DrawDynamicSpectrum ();

		DrawDynamicOutput ();


		//float[] dataSamplesL = aSource.GetOutputData (64, 0);
		//float[] dataSamplesR = aSource.GetOutputData (64, 1);


		//Debug.Log ("data sampling = " + dataSamplesL[0]);

	}

	private void DrawDynamicSpectrum()
	{


		for(int i=0; i<4;i++)
		{
			cubePos.Set(cubesTransform[i].position.x, Mathf.Clamp(samples[i] * (50+i*i), 0, 50), cubesTransform[i].position.z);

			if(cubePos.y >= cubesTransform[i].position.y)
				cubesTransform[i].position = cubePos;
			else
				cubesTransform[i].position -= gravity;



		}


	}

	private void DrawDynamicOutput()
	{
		//Channel 1
		aSource.GetOutputData (samplesL, 0);

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

		DrawSingleCube(0, average);
		DrawSingleCube(1, max);


		//Channel 2
		aSource.GetOutputData (samplesL, 1);

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

		//Debug.Log ("average = " + average.ToString());


		DrawSingleCube(3, average);
		DrawSingleCube(2, max);



	}

	private void DrawSingleCube(int index, float value)
	{
		cubePos.Set(cubesTransform[index].position.x, Mathf.Clamp(value * 50, 0, 50), cubesTransform[index].position.z);

		if(cubePos.y >= cubesTransform[index].position.y)
			cubesTransform[index].position = cubePos;
		else
			cubesTransform[index].position -= gravity;
				
	}
					

	private void GetAudioSamples()
	{

		float[] samples = new float[aSource.clip.samples * aSource.clip.channels];
		aSource.clip.GetData (samples, 0);
		int i = 0;
		while (i < samples.Length) {
			samples [i] = samples [i] * 0.5F;
			++i;
		}

		Debug.Log ("Sample Len = " + samples.Length);

	}




}
