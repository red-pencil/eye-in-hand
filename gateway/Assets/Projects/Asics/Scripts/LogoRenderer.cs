using UnityEngine;
using System.Collections;

public class LogoRenderer : MonoBehaviour {

	public Texture TargetLogo;
	public float Size=100;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (20,Camera.main.pixelHeight-Size-20,Size,Size),TargetLogo);
	}
}
