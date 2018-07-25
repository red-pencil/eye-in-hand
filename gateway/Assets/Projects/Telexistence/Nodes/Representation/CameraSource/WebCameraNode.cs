using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Klak.Wiring;

[ModelBlock("Representation/Vision/Webcamera Block","Webcamera",120)]
public class WebCameraNode : BlockBase {

    [SerializeField]
	int _index=0;

    //LocalWebcameraSource _camSource;
    LocalCameraVideoPlayer _camSource;

	[SerializeField, Outlet]
	TextureEvent texture;

    public int Width = 640;
    public int Height = 480;

    public bool ProcessedImage = false;

    [Inlet]
    public int Index
    {
        set
        {
            if (value == _index)
                return;
            _index = value;
        }
        get
        {
            return _index;
        }
    }

    // Use this for initialization
    void Start () {
		_camSource = gameObject.AddComponent< LocalCameraVideoPlayer >();
        //	_camSource.Init (idx);
        _camSource.CameraIndex = -1;

	}

	void OnDestroy()
	{
		_camSource.StopPlayer ();
	}
		
	// Update is called once per frame
	protected override void UpdateState() {
		if (Index != _camSource.CameraIndex)
        {
            _camSource.StopPlayer();
            _camSource.CameraIndex = _index;
            _camSource.Width = Width;
            _camSource.Height = Height;
            _camSource.StartPlayer ();
		}

		if (texture.GetPersistentEventCount () > 0) {
            if(ProcessedImage)
			    texture.Invoke (_camSource.VideoTexture);
            else
                texture.Invoke(_camSource.SourceTexture);
        }

	}
}
