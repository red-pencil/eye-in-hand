using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TxGeneralImageProcessor:ITxEyesImageProcessor {
	
	ICameraSource _texture;
	TxKitEyes _eyes;
	OffscreenProcessor[] _SrcBlitter;

	public TxGeneralImageProcessor(TxKitEyes e)
	{
		_eyes = e;

	}

	public override void PostInit()
	{
		_texture=_eyes.CameraSource;
	}
	public override void ProcessMainThread (ref TxVisionOutput result)
	{
		if (result.Configuration.SeparateStreams) {
			for (int i = 0; i < _texture.GetTexturesCount (); ++i) {
				result.SetTexture (_texture.GetEyeTexture (i), i);
			}
		} else {
			if (_texture.GetTexturesCount () == 0)
				return;

			if (_SrcBlitter == null || _SrcBlitter.Length != result.Configuration.CameraStreams) {
				_SrcBlitter =new OffscreenProcessor[result.Configuration.CameraStreams];
				for (int i = 0; i < result.Configuration.CameraStreams; ++i) {
					_SrcBlitter [i] = new OffscreenProcessor ();
					_SrcBlitter[i].ShaderName = "Image/Blitter";
					_SrcBlitter[i].TargetFormat = RenderTextureFormat.Default;
					_SrcBlitter[i].TargetFormat = RenderTextureFormat.ARGB32;
                    _SrcBlitter[i].ProcessingMaterial.SetInt("Flip", 1);
                }
            }
			Rect texRect = new Rect (0, 0, 1, 1.0f/(float)result.Configuration.CameraStreams);
			var tex=_texture.GetEyeTexture (0);
            if (tex == null)
                return;
            result.SetSourceTexture(tex, 0);
            for (int i = 0; i < result.Configuration.CameraStreams; ++i) {
				_SrcBlitter[i].ProcessingMaterial.SetVector ("TextureRect", new Vector4(texRect.x, texRect.y, texRect.width, texRect.height));

				var t= _SrcBlitter[i].ProcessTextureSized (tex,(int)(texRect.width*tex.width),(int)(texRect.height*tex.height));//CamTexture;//
				result.SetTexture (t, i);

                texRect.y += texRect.height;
			}
		}
	}
	public override bool RequireCameraCorrection(){
		return true;
	}
}
