using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;
using System;

[ModelBlock("Transfer/Vision/Color Correction")]
public class ColorCorrectionNode : BlockBase {

	RenderTexture _resultTexture;
    ColorCorrectionImageProcessor _processor;


	[SerializeField, Inlet]
	public Texture Input
	{
		set {
            _processor.ProcessTexture(value, ref _resultTexture);
            _result.Invoke(_resultTexture);
		}
	}
	[SerializeField, Outlet]
	TextureEvent _result;
	// Use this for initialization
	void Start () {
        _processor = new ColorCorrectionImageProcessor();

	}
	
	// Update is called once per frame
	void Update () {
	}
}
