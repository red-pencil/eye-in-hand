using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;

[ModelBlock("TxKit/Transfer/Eye Stitch")]
public class EyeStitchNode : BlockBase {

	Texture _left,_right;
	RenderTexture _output;
	float _focus,_size;

	[SerializeField, Outlet]
	TextureEvent Output=new TextureEvent();

	[Inlet]
	public Texture Left{
		set{
			if (!enabled) return;
			_left = value;
		}
	}
	[Inlet]
	public Texture Right{
		set{
			if (!enabled) return;
			_right = value;
		}
	}
	[Inlet]
	public float Focus
	{
		set{
			_focus = value;
		}
	}
	[Inlet]
	public float Size
	{
		set{
			_size = value;
		}
	}


	OffscreenProcessor _processor;


	void Start()
	{
		_processor=new OffscreenProcessor();
		_processor.ShaderName = "Leon/EyeBlender";

		_output = new RenderTexture (1920, 1080, 24, RenderTextureFormat.Default);
	}

	protected override void UpdateState()
	{ 
		RenderTexture.active = _output;
		GL.PushMatrix ();
		GL.LoadPixelMatrix (0, 1920, 1080, 0);
		GL.Clear (false, true, Color.black);
		_processor.ProcessingMaterial.SetFloat ("_Focus",_focus);
		_processor.ProcessingMaterial.SetFloat ("_Size",_size);


		if (_left != null) {
			_processor.ProcessingMaterial.SetFloat ("_flip",0);
			Graphics.DrawTexture (new Rect (0, 0, 1920, 1080), _processor.ProcessTexture (_left,0));
		}
		if (_right != null) {
			_processor.ProcessingMaterial.SetFloat ("_flip",1);
			Graphics.DrawTexture (new Rect (0, 0, 1920, 1080), _processor.ProcessTexture (_right,0));
		}
		GL.PopMatrix ();
		RenderTexture.active = null;

		Output.Invoke (_output);
	}

}