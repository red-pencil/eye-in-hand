using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ITxEyesImageProcessor {

	public virtual void ProcessTextures(ref TxVisionOutput result)
	{}
	public virtual void ProcessMainThread(ref TxVisionOutput result){
	}

	public virtual GstImageInfo.EPixelFormat GetPixelFormat(){
		return GstImageInfo.EPixelFormat.EPixel_I420;
	}

	public virtual void PostInit(){}

	public virtual void Destroy(){}
	public virtual void Invalidate(){}


	public virtual bool RequireCameraCorrection(){
		return false;
	}
}
