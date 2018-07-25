using UnityEngine;
using System.Collections;

public class TelubeeCameraAVProvider : AVProvider {

	protected ICameraSource _camSource;
	protected IAudioSource _audioSource;
	public TxKitEyes TargetEyes;
	public TxKitEars TargetEars;
	// Use this for initialization
	void Start () {

		if (TargetEyes == null) {
			TargetEyes = GetComponent<TxKitEyes> ();
		}
		if (TargetEyes == null) {
			UnityEngine.Debug.LogError("TargetEyes is not assigned!");
			return;
		}


		TargetEyes.OnCameraSourceCreated+=_OnCameraSourceCreated;
		TargetEyes.OnCameraRendererCreated += _OnCameraRendererCreated;
		TargetEyes.OnImageArrived += _OnImageArrived;

		TargetEars.OnAudioSourceCreated += _OnAudioSourceCreated;
	}

	void _OnImageArrived(TxKitEyes src,int eye)
	{
		if (OnImageArrived != null)
			OnImageArrived (this, eye);
	}
	void _OnCameraRendererCreated(TxKitEyes creator,ICameraRenderMesh[] renderers)
	{
		if (OnCameraRendererCreated != null)
			OnCameraRendererCreated (this, renderers);
	}

	void _OnCameraSourceCreated(TxKitEyes creator,ICameraSource src)
	{
		if (src == null)
			return;
		_camSource = src;
	}
	void _OnAudioSourceCreated(TxKitEars creator,IAudioSource src)
	{
		if (src == null)
			return;
		_audioSource = creator.AudioSource;
	}

	// Update is called once per frame
	void Update () {
	
	}


	public override Texture GetTexture(int stream)
	{
		if(TargetEyes.CamRenderer.Length>stream && TargetEyes.CamRenderer[stream]!=null)
			return TargetEyes.CamRenderer[stream].GetTexture();
		return null;
	}/*
	public override Texture GetRawTexture(int stream)
	{
		if(TargetEyes.CamRenderer.Length>stream && TargetEyes.CamRenderer[stream]!=null)
			return TargetEyes.CamRenderer[stream].GetRawTexture();
		return null;
	}*/
	public override float GetAudioLevel()
	{
		if(_audioSource != null) {
			return  _audioSource.GetAverageAudioLevel();
		}
		return 0;
	}
	public override void SetAudioLevel(float level)
	{
		if (_audioSource != null)
			_audioSource.SetAudioVolume(level);
	}
}
