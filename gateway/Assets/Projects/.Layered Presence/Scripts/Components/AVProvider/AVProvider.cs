using UnityEngine;
using System.Collections;

public class AVProvider : MonoBehaviour {

	public delegate void OnCameraRendererCreated_deleg(AVProvider provider,ICameraRenderMesh[] renderers);
	public OnCameraRendererCreated_deleg OnCameraRendererCreated;

	public delegate void OnImageArrived_deleg(AVProvider src,int eye);
	public OnImageArrived_deleg OnImageArrived;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public virtual Texture GetTexture(int stream)
	{
		return null;
	}/*
	public virtual Texture GetRawTexture(int stream)
	{
		return null;
	}*/
	public virtual float GetAudioLevel()
	{
		return 0;
	}

	public virtual void SetAudioLevel(float level)
	{

	}
}
