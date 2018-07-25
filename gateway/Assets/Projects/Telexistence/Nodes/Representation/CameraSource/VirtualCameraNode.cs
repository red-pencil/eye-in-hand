using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Klak.Wiring;

[ModelBlock("Representation/Vision/Virtual Camera","",120)]
public class VirtualCameraNode : BlockBase {

	public Camera CamSource;

	[SerializeField, Outlet]
	TextureEvent texture;

    public int Width = 640;
    public int Height = 480;

    TextureWrapper _wrapper=new TextureWrapper();

    // Use this for initialization
    void Start () {
	}
    
		
	// Update is called once per frame
	void UpdateState () {

        if (CamSource == null)
            return;
        if(CamSource.targetTexture==null)
        {
            CamSource.targetTexture = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        }

		if (texture.GetPersistentEventCount () > 0) {
            var t = _wrapper.ConvertTexture(CamSource.targetTexture);
            texture.Invoke (t);
		}

	}
}
