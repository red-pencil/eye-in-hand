using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class TxOVRFoveatedRenderingImageProcessor:TxFoveatedRenderingImageProcessor  {


	class CustomOvrvisionProcessor:TxOVRVisionImageProcessor 
	{
		public Texture2D processedTex;
		IntPtr _dataPtr=IntPtr.Zero;
		byte[] dataArray;
		Color[] pixels=null;

		public CustomOvrvisionProcessor(TxKitEyes e):base(e)
		{
		}
		public override void Destroy()
		{
			base.Destroy ();
			Marshal.FreeHGlobal (_dataPtr);
			_dataPtr = IntPtr.Zero;
		}
		public override void Invalidate()
		{
			base.Invalidate ();
			Marshal.FreeHGlobal (_dataPtr);
			_dataPtr = IntPtr.Zero;
		}

		protected override IntPtr GetTexturePtr()
		{
			return IntPtr.Zero;
			//	return GStreamerCore.GetTextureData (processedTex);
			if (_dataPtr == IntPtr.Zero) {
				pixels= processedTex.GetPixels ();
				dataArray = new byte[pixels.Length];
				_dataPtr = Marshal.AllocHGlobal (dataArray.Length);
			}
			for (int i = 0; i < pixels.Length; ++i) {
				dataArray [i] = (byte)(pixels [i].r*255);
			}
			Marshal.Copy (dataArray, 0, _dataPtr, dataArray.Length);
			return _dataPtr;
		}
		protected override void  ReleaseTexturePtr()
		{
		//	GStreamerCore.ReleaseTextureData(processedTex);
		}

	}

	CustomOvrvisionProcessor _ovrProcessor;


	OffscreenProcessor[] _SrcBlitter=new OffscreenProcessor[4];

	RenderTexture CombinedTexture;

	Material _blitMtrl;

	RenderTextureWrapper _wrapper=new RenderTextureWrapper();

	public TxOVRFoveatedRenderingImageProcessor(TxKitEyes e):base(e)
	{
		_ovrProcessor = new CustomOvrvisionProcessor (e);
		for (int i = 0; i < 4; ++i) {
			_SrcBlitter[i] = new OffscreenProcessor ();
			_SrcBlitter[i].ShaderName = "Image/Blitter";
			_SrcBlitter[i].TargetFormat = RenderTextureFormat.Default;
			_SrcBlitter[i].TargetFormat = RenderTextureFormat.ARGB32;
		}

		_blitMtrl = new Material (Shader.Find("Image/Blitter"));
		_blitMtrl.SetPass (1);

		CombinedTexture = null;
	}

	public override void PostInit()
	{
		base.PostInit ();
		_ovrProcessor.PostInit ();
	}
	public override void ProcessTextures (ref TxVisionOutput result)
	{
		base.ProcessTextures (ref result);
		_ovrProcessor.ProcessTextures (ref result);
	}
	public override void ProcessMainThread (ref TxVisionOutput result)
	{
		if (_texture.StreamsCount == 0)
			return;

		Parameters.frameSize = result.Configuration.FrameSize;

		Texture srcTex= _texture.GetEyeTexture (0);
		if (srcTex == null)
			return;

		Texture[] tex = new Texture[4];
		Texture[] tex2 = new Texture[4];
		Rect texRect = new Rect (0, 0, 1, 0.25f);
		for (int i = 0; i < 4; ++i) {
			_SrcBlitter[i].ProcessingMaterial.SetVector ("TextureRect", new Vector4 (texRect.x, texRect.y, texRect.width, texRect.height));

			tex[i] = _SrcBlitter[i].ProcessTextureSized (srcTex,(int)(texRect.width*srcTex.width),(int)(texRect.height*srcTex.height));//CamTexture;//

			texRect.y += 0.25f;
		}
		InitResults (4);

		for (int i = 0; i < 4; ++i) {
			tex2[i] = BlitImage (tex[i],i,new Vector2(1,0.5f));
		}

		//combine textures
		if (CombinedTexture == null || CombinedTexture.width != Parameters.frameSize.x * 2 || CombinedTexture.height != Parameters.frameSize.y) {
			CombinedTexture = new RenderTexture ((int)Parameters.frameSize.x * 2, (int)Parameters.frameSize.y,24,RenderTextureFormat.Default);
		}
		RenderTexture.active = CombinedTexture;
		GL.Clear (true, true, Color.black);
		GL.PushMatrix ();
		GL.LoadPixelMatrix (0, CombinedTexture.width, CombinedTexture.height, 0);
		Graphics.DrawTexture (new Rect (0, 0, tex2[0].width,tex2[0].height), tex2[0],_blitMtrl);
		Graphics.DrawTexture (new Rect (tex2[0].width, 0, tex2[0].width,tex2[0].height), tex2[1],_blitMtrl);
		Graphics.DrawTexture (new Rect (0, tex2[0].height, tex2[0].width,tex2[0].height), tex2[2],_blitMtrl);
		Graphics.DrawTexture (new Rect (tex2[0].width, tex2[0].height, tex2[0].width,tex2[0].height), tex2[3],_blitMtrl);

		GL.PopMatrix ();
		RenderTexture.active = null;

		result.SetTexture (CombinedTexture, 0);

		//Texture2D resultTex= _wrapper.ConvertRenderTexture (CombinedTexture);
		//_ovrProcessor.processedTex = resultTex;
		//_ovrProcessor.ProcessMainThread (ref result);

	}

}
