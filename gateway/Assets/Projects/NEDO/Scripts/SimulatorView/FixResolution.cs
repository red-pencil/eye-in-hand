using UnityEngine;
using System.Collections;

public class FixResolution : MonoBehaviour {

	public int Width=480;
	public int Height=270;
	// Use this for initialization
	void Start () {

		Width=int.Parse(Settings.Instance.GetValue ("Simulator", "Width", Width.ToString ()));
		Height=int.Parse(Settings.Instance.GetValue ("Simulator", "Height", Height.ToString ()));
		Screen.SetResolution (Width,Height,false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
