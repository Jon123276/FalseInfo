using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FalseInfo : MonoBehaviour {

	// Use this for initialization
	public KMBombInfo bomb;
	public KMBombModule module;
	public KMAudio audio;
	public GameObject background;
	public KMSelectable lieDetect;
	public AudioClip[] clips;
	public Material mat;
	public Material back;
	public Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta};
	private string[] Transforms = { "Sound", "SphereC", "BackgroundC"};
	private List<string> truthKeys = new List<string>();
	private List<string> truthValues = new List<string>();
	private List<Color> colorTruths = new List<Color>();
	private int whereIsLie = 0;
	public GameObject[] spheres;
	private static int _moduleIdCounter = 1;
	private int moduleId;
	public bool isSolved;
	private int currentTruth = 0;
	private int currentTime = 0;
	private int solveTime = 0;
	void DebugFunc(string step)
	{
		if (step == "start")
		{
			for (int i = 0; i < truthValues.Count; i++)
			{
				Debug.LogFormat("[False Info #{0}] Transform {1}: {2} as {3}", moduleId, i + 1, truthKeys[i], truthValues[i]);
			}
			Debug.Log("Is this equal? " + (truthKeys.Count() == truthValues.Count()));
		}
	}
	void reset()
    {
		foreach (GameObject sphere in spheres)
        {
			sphere.GetComponent<MeshRenderer>().material.color = Color.white;
			background.GetComponent<MeshRenderer>().material = back;
		}
    }
	IEnumerator valueInc()
    {
		while (true)
		{
			currentTime++;
			Debug.Log("Current value: " + currentTime);
			if (currentTime == 5)
            {
				currentTime = 0;
				bool lying = false;
				if (Random.Range(0, 3) == 0) lying = true;
				StartCoroutine(timeEvents(lying));
				yield return new WaitForSeconds(6f);
            }
			yield return new WaitForSeconds(1f);
		}
    }

	void someFunc(int i)
    {
		if (truthKeys[i] == "SphereC")
		{
			foreach (GameObject sphere in spheres)
			{
				sphere.GetComponent<MeshRenderer>().material.color = colorTruths[currentTruth];
			}
			currentTruth++;
		}
		else if (truthKeys[i] == "BackgroundC")
		{
			mat.color = colorTruths[currentTruth];
			background.GetComponent<MeshRenderer>().material = mat;
			currentTruth++;
		}
		else if (truthKeys[i] == "Sound")
		{
			audio.PlaySoundAtTransform(truthValues[i], spheres[0].transform);
		}
		currentTruth %= 2;
	}

	void lieFunc(int i)
    {
		if (truthKeys[i] == "SphereC")
		{
			foreach (GameObject sphere in spheres)
			{
				sphere.GetComponent<MeshRenderer>().material.color = colorTruths[Random.Range(0, colorTruths.Count())];
			}
		}
		else if (truthKeys[i] == "BackgroundC")
		{
			mat.color = colorTruths[Random.Range(0, colorTruths.Count())];
			background.GetComponent<MeshRenderer>().material = mat;
		}
		else if (truthKeys[i] == "Sound")
		{
			audio.PlaySoundAtTransform(truthValues[Random.Range(0, clips.Length)], spheres[0].transform);
		}
	}
	IEnumerator timeEvents(bool lying = false)
    {
		for (int i = 0; i < truthKeys.Count; i++)
        {
			if (lying && i == whereIsLie)
			{
				Debug.LogFormat("[Falsifying Info #{0}] The module is currently lying about index {1}.", moduleId, i);
				lieFunc(i);
            } else
            {
				Debug.LogFormat("[Falsifying Info #{0}] The module is at {1}.", moduleId, i);
				solveTime++;
				someFunc(i);
			}
			yield return new WaitForSeconds(1f);
		}
		solveTime = 0;
		currentTruth = 0;
		reset();
    }
	void Awake()
    {
		moduleId++;
	}
	void Start () {
		whereIsLie = Random.Range(0, 5);
		for (int i = 0; i < 5; i++)
        {
			truthKeys.Add(Transforms[Random.Range(0, Transforms.Length)]);
			if (truthKeys[i] == "SphereC" || truthKeys[i] == "BackgroundC")
            {
				Color tempColor = colors[Random.Range(0, colors.Length)];
				truthValues.Add(tempColor.ToString());
				colorTruths.Add(tempColor);
            }
			else if (truthKeys[i] == "Sound")
            {
				truthValues.Add(clips[Random.Range(0, clips.Length)].ToString());
            }
        }
		lieDetect.OnInteract += delegate
		{
			StopAllCoroutines();
			Debug.LogFormat("[Falsifying Info #{0}] The time submitted is {1}.", moduleId, solveTime);
			if (solveTime == 0 || solveTime != whereIsLie)
			{
				module.HandleStrike();
				solveTime = 0;
				StartCoroutine(valueInc());
				reset();
			}
			else module.HandlePass();
			return false;
		};
		DebugFunc("start");
		StartCoroutine(valueInc());

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
