using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TxFoveatedRenderingImageProcessor : ITxEyesImageProcessor {


	[Serializable]
	public class FoveatedRendererParameters
	{
		public EyegazeDataStreamer.GazeBlitType BlitType=EyegazeDataStreamer.GazeBlitType .Circular;
		public bool DrawRectangle=false;
		public bool DebugBlitArea=false;
		public float BlurSize=1;
		public Vector2 frameSize;
		public Vector2 BlitParameter=new Vector2(0.3f,0.5f);
		public bool[] DisabledFoveations=new bool[10];
	}
	[Serializable]
	public class FoveatedRendererResults
	{
		public FoveatedRendererResults()
		{
			CombinedTexture = new RenderTexture ((int)1, (int)1, 16, RenderTextureFormat.ARGB32);
			GazeTexture = new Texture[0];
			ProcessedGazeTexture=new Texture[0];
		}
		public Texture OriginalTexture;
		public Texture SceneTexture;
		public Texture CorrectedTexture;
		public RenderTexture CombinedTexture;
		public Texture[] GazeTexture;
		public Texture[] ProcessedGazeTexture;
		public Vector4[] EyeGaze;
		public Vector2 gazeSize;

		public OffscreenProcessor _SceneBlitter;
	}

	public FoveatedRendererParameters Parameters=new FoveatedRendererParameters();
	public FoveatedRendererResults[] Results;


	TxKitEyes _eyes;
	protected MultipleNetworkCameraSource _texture;
	//OffscreenProcessor _SrcBlitter;
	OffscreenProcessor _GazeBlitterRect;
	OffscreenProcessor _GazeBlitterCircle;
	OffscreenProcessor _GazeBlitter;

	bool m_dirty=false;
	bool m_foveatedStreaming=false;

	OffscreenProcessor _SceneBlur;

	OffscreenProcessor[] _SrcBlitter;
	Material _blitMtrl;

	public delegate void OnFoveatedResultsChanged_Delg(TxFoveatedRenderingImageProcessor src,FoveatedRendererResults[] results);
	public event OnFoveatedResultsChanged_Delg OnFoveatedResultsChanged;

	public TxFoveatedRenderingImageProcessor(TxKitEyes e)
	{
		_eyes = e;
		/*
		_SrcBlitter=new OffscreenProcessor();
		_SrcBlitter.ShaderName = "Image/Blitter";
		_SrcBlitter.TargetFormat = RenderTextureFormat.Default;
		_SrcBlitter.TargetFormat = RenderTextureFormat.ARGB32;
*/
		_GazeBlitterCircle=new OffscreenProcessor();
		_GazeBlitterCircle.ShaderName= "Image/GazeBlit_Circle";
		_GazeBlitterCircle.TargetFormat = RenderTextureFormat.ARGB32;

		_GazeBlitterRect=new OffscreenProcessor();
		_GazeBlitterRect.ShaderName= "Image/GazeBlit_Rect";
		_GazeBlitterRect.TargetFormat = RenderTextureFormat.ARGB32;

		_GazeBlitter=new OffscreenProcessor();
		_GazeBlitter.ShaderName = "Image/Blitter";
		_GazeBlitter.TargetFormat = RenderTextureFormat.ARGB32;


		//		CamSource.GetBaseTexture ().OnFrameBlitted+= OnFrameGrabbed;

		_blitMtrl = new Material (Shader.Find("Image/Blitter"));
		_blitMtrl.SetPass (1);
	}

	public override void PostInit()
	{
		_texture=(_eyes.CameraSource as MultipleNetworkCameraSource);
	}

	protected void InitResults(int count)
	{
		if (Results == null || Results.Length < count) {
			Results = new FoveatedRendererResults[count];
			for (int i = 0; i < Results.Length; ++i) {
				Results [i] = new FoveatedRendererResults ();

				Results [i] ._SceneBlitter=new OffscreenProcessor();
				Results [i] ._SceneBlitter.ShaderName = "Image/Blitter";
				Results [i] ._SceneBlitter.TargetFormat = RenderTextureFormat.Default;
			}

			if (OnFoveatedResultsChanged != null)
				OnFoveatedResultsChanged (this, Results);
		}
	}
	public override void ProcessMainThread (ref TxVisionOutput result)
	{
		if (_texture.StreamsCount == 0)
			return;
		Parameters.frameSize = result.Configuration.FrameSize;
		InitResults (_texture.CameraStreams);

		if (_SrcBlitter == null || _SrcBlitter.Length != result.Configuration.CameraStreams) {
			_SrcBlitter =new OffscreenProcessor[result.Configuration.CameraStreams];
			for (int i = 0; i < result.Configuration.CameraStreams; ++i) {
				_SrcBlitter [i] = new OffscreenProcessor ();
				_SrcBlitter[i].ShaderName = "Image/Blitter";
				_SrcBlitter[i].TargetFormat = RenderTextureFormat.Default;
				_SrcBlitter[i].TargetFormat = RenderTextureFormat.ARGB32;
			}
		}

		Rect texRect = new Rect (0, 0, 1, 1.0f/_texture.CameraStreams);
		var srcTex = _texture.GetEyeTexture (0);
		if (srcTex == null)
			return;
		Texture tex = srcTex;
		result.SetSourceTexture (srcTex, 0);
		for (int i = 0; i < _texture.CameraStreams; ++i) {
			if (_texture.CameraStreams > 1) {
				_SrcBlitter [i].ProcessingMaterial.SetVector ("TextureRect", new Vector4 (texRect.x, texRect.y, texRect.width, texRect.height));
				tex = _SrcBlitter[i].ProcessTextureSized (srcTex,(int)(texRect.width*srcTex.width),(int)(texRect.height*srcTex.height));
				texRect.y += texRect.height;
			}
			var t = BlitImage (tex,i,new Vector2(1,1));
			if(t!=null)
				result.SetTexture(t,i);
		}
	}


	Color[] ColorWheel = new Color[] {
		Color.red,
		Color.blue,
		Color.green,
		Color.cyan
	};


	protected Texture BlitImage(Texture src,int idx,Vector2 Scaler)
	{
		m_dirty = false;
	//	Texture src = _texture.GetEyeTexture(idx);
		Results[idx].OriginalTexture = src;
		//ulong frame = CamSource.GetGrabbedBufferID ((int)Eye);
		//if (m_dirty) 
		//_lastFrame = frame;

		int levels = _texture.GetEyeGazeLevels ();
		m_foveatedStreaming = (levels > 0);
		if (Results[idx].EyeGaze == null || Results[idx].EyeGaze.Length != levels)
			Results[idx].EyeGaze = new Vector4[levels];
		for (int i = 0; i < levels; ++i) {
			Results[idx].EyeGaze[i] = _texture.GetEyeGaze (0,idx, i);

		}

		Vector2 frameSize = Parameters.frameSize;
		frameSize.x *= Scaler.x;
		frameSize.y *= Scaler.y;

		if (frameSize.x != Results[idx].CombinedTexture.width ||
			frameSize.y != Results[idx].CombinedTexture.height) {
			Results[idx].CombinedTexture = new RenderTexture ((int)frameSize.x, (int)frameSize.y, 16, RenderTextureFormat.Default);
		}
		Rect texRect = new Rect (0, 0, 1, 1);
		if (src == null)
			return null;

		Results[idx].gazeSize.x = src.height;
		Results[idx].gazeSize.y = src.height;

		//Get Foveation levels
		if (levels > 0) {
			//blit scene texture
			Results [idx] ._SceneBlitter.ProcessingMaterial.SetVector ("TextureRect", new Vector4 ((Results[idx].gazeSize.x * levels) / (float)src.width, 0, ((float)src.width - Results[idx].gazeSize.x * levels) / (float)src.width, texRect.height));
			Results[idx].SceneTexture = Results [idx] ._SceneBlitter.ProcessTextureSized (src, (int)(src.width - Results[idx].gazeSize.x * levels), src.height);//CamTexture;//
			if (true) {
				RenderTexture.active = Results[idx].CombinedTexture;
				GL.Clear (true, true, Color.black);
				GL.PushMatrix ();
				GL.LoadPixelMatrix (0, Results[idx].CombinedTexture.width, Results[idx].CombinedTexture.height, 0);

				Graphics.DrawTexture (new Rect (0, 0, frameSize.x, frameSize.y), Results[idx].SceneTexture);



				var blitter = _GazeBlitterCircle;

				switch (Parameters.BlitType) {
				case EyegazeDataStreamer.GazeBlitType.Circular:
					blitter = _GazeBlitterCircle;
					break;
				case EyegazeDataStreamer.GazeBlitType.Rectangular:
					blitter = _GazeBlitterRect;
					break;
				}

				if (Results [idx].GazeTexture.Length != levels) {
					Results [idx].GazeTexture = new Texture[levels];
					Results [idx].ProcessedGazeTexture = new Texture[levels];
				}

				//blit all gaze levels
				for (int i = levels - 1; i >= 0; --i) {
					if (Parameters.DisabledFoveations [i])
						continue;
					//blit gaze texture
					_GazeBlitter.ProcessingMaterial.SetVector ("TextureRect", new Vector4 (Results[idx].gazeSize.x * i / (float)src.width, 0, Results[idx].gazeSize.x / (float)src.width, texRect.height));
					Results[idx].GazeTexture [i] = _GazeBlitter.ProcessTextureSized (src, (int)Results[idx].gazeSize.x, (int)Results[idx].gazeSize.y);//CamTexture;//

					var clr=ColorWheel [i % ColorWheel.Length];

					blitter.ProcessingMaterial.SetFloat ("DebugBlitArea", Parameters.DebugBlitArea ? 1 : 0);
					blitter.ProcessingMaterial.SetVector ("_DebugColor", new Vector4 (clr.r,clr.g,clr.b,clr.a));
					blitter.ProcessingMaterial.SetVector ("_Parameters", new Vector4 (0.5f, 0.5f, Parameters.BlitParameter.x, Parameters.BlitParameter.y));
					Results[idx].ProcessedGazeTexture[i]= blitter.ProcessTexture (Results [idx].GazeTexture [i]);

					Graphics.DrawTexture (new Rect (Results[idx].EyeGaze[i].x, Results[idx].EyeGaze[i].y, Results[idx].EyeGaze[i].z, Results[idx].EyeGaze[i].w), Results[idx].ProcessedGazeTexture [i]);

				}

				if (Parameters.DrawRectangle) {
					for (int i = 0; i < levels; ++i) {

						if (Parameters.DisabledFoveations [i])
							continue;
						var r=new Rect (Results[idx].EyeGaze [i].x, Results[idx].EyeGaze [i].y, Results[idx].EyeGaze [i].z, Results[idx].EyeGaze [i].w);
						if(Parameters.BlitType== EyegazeDataStreamer.GazeBlitType.Rectangular)
							GUITools.GraphicsDrawScreenRectBorder (r, 4, ColorWheel [i % ColorWheel.Length]);
						else 
							GUITools.DrawCircle (r.center,r.height/2, 90, ColorWheel [i % ColorWheel.Length],3);
					}
				}
				GL.PopMatrix ();
				RenderTexture.active = null;
			}else
				Results[idx].CombinedTexture=Results[idx].SceneTexture as RenderTexture;
		} else {
			Results [idx] ._SceneBlitter.ProcessingMaterial.SetVector ("TextureRect", new Vector4 (0, 0,1,1));
			Results[idx].SceneTexture = Results [idx] ._SceneBlitter.ProcessTextureSized (src, src.width , src.height);//CamTexture;//

			Results[idx].CombinedTexture=Results[idx].SceneTexture as RenderTexture;
		}

		return Results[idx].CombinedTexture;
	}

	public override bool RequireCameraCorrection(){
		return true;
	}
}
