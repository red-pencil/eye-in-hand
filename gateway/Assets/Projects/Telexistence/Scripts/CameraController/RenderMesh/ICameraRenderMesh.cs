using UnityEngine;
using System.Collections;

public abstract class ICameraRenderMesh:ICameraPostRenderer {

	public EyeName Eye;
	public Material Mat;
	public GameObject _RenderPlane;
	public TxEyesRenderer Src;
	public Camera DisplayCamera;

	public abstract void RequestDestroy ();
	public abstract void CreateMesh (EyeName eye);
	public abstract void ApplyMaterial (Material m);
	public abstract void Enable ();
	public abstract void Disable ();

	public abstract Texture GetTexture();
	//public abstract Texture GetRawTexture();

	//public abstract void SetRotationOffset(Quaternion q);
}
