using UnityEngine;
using System.Collections;
using System.Threading;

public class WebcameraRenderMesh : CustomRenderMesh {
	
	protected override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public override void RequestDestroy ()
	{
		base.RequestDestroy ();
		Destroy (_RenderPlane);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy ();
	}


	public override void Enable()
	{
		if (_RenderPlane == null)
			return;
		MeshRenderer mr = _RenderPlane.GetComponent<MeshRenderer> ();
		if (mr != null) {
			mr.enabled=true;
		}
		this.enabled = true;
	}
	public override void Disable()
	{
		if (_RenderPlane == null)
			return;
		MeshRenderer mr = _RenderPlane.GetComponent<MeshRenderer> ();
		if (mr != null) {
			mr.enabled=false;
		}
		this.enabled = false;
	}

	protected override MeshRenderer _internalCreateMesh(EyeName eye)
	{
		int i = (int)eye;
		if(_RenderPlane==null)
			_RenderPlane = new GameObject("EyesRenderPlane_"+eye.ToString());
		MeshFilter mf = _RenderPlane.GetComponent<MeshFilter> ();
		if (mf == null) {
			mf = _RenderPlane.AddComponent<MeshFilter> ();
		}
		MeshRenderer mr = _RenderPlane.GetComponent<MeshRenderer> ();
		if (mr == null) {
			mr = _RenderPlane.AddComponent<MeshRenderer> ();
		}
		
		mr.material = Mat;
		mf.mesh.vertices = new Vector3[]{
			new Vector3(-1,-1,1),
			new Vector3( 1,-1,1),
			new Vector3( 1, 1,1),
			new Vector3(-1, 1,1)
		};
		Rect r = new Rect(0,0,1,1);// CamSource.GetEyeTextureCoords (eye);
		Vector2[] uv = new Vector2[]{
			new Vector2(r.x,r.y),
			new Vector2(r.x+r.width,r.y),
			new Vector2(r.x+r.width,r.y+r.height),
			new Vector2(r.x,r.y+r.height),
		};
        Matrix4x4 rotMat = Matrix4x4.identity;
        if (Src.Output.Configuration.Rotation[i] == CameraConfigurations.ECameraRotation.Flipped)
        {
            rotMat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 180), Vector3.one);
        }
        else if (Src.Output.Configuration.Rotation[i] == CameraConfigurations.ECameraRotation.CW)
        {
            rotMat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90), Vector3.one);
        }
        else if (Src.Output.Configuration.Rotation[i] == CameraConfigurations.ECameraRotation.CCW)
        {
            rotMat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, -90), Vector3.one);
        }
        for (int v = 0; v < 4; ++v)
        {
            Vector3 res=rotMat*(2*uv[v]-Vector2.one);
			uv[v]=(new Vector2(res.x,res.y)+Vector2.one)*0.5f;//Vector2.one-uv[v];
			if(Src.Output.Configuration.FlipXAxis)
			{
				uv[v].x=1-uv[v].x;
			}
			if(Src.Output.Configuration.FlipYAxis)
			{
				uv[v].y=1-uv[v].y;
			}
		//	uv[v].y=1-uv[v].y;
		}
		mf.mesh.uv = uv;
		mf.mesh.triangles = new int[]
		{
			0,2,1,0,3,2
		};
		
		_RenderPlane.transform.localPosition =new Vector3 (0, 0, 0);
		if (Eye == EyeName.LeftEye)
			_RenderPlane.transform.localPosition = new Vector3 (-0.032f, 0, 0);
		else 
			_RenderPlane.transform.localPosition = new Vector3 (0.032f, 0, 0);
		_RenderPlane.transform.localRotation =Quaternion.identity;

		return mr;
	}
}
