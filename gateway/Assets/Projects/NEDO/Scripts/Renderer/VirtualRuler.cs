using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VirtualRuler : MonoBehaviour {

	public Camera TargetCamera;
	public float Distance;
	public float Size;
	public Material RenderMaterial;
	bool showText = true;

	float _lastSize;

	public Text DistanceLabel;
	public Text SizeLabel;

	bool _Visible=false;

	public Vector2 _distancePos,_sizePos;

	Text _displayText;
	// Use this for initialization
	void Start () {
		MeshRenderer r= gameObject.AddComponent<MeshRenderer> ();
		MeshFilter mf= gameObject.AddComponent<MeshFilter> ();

		//_lastSize = Size;
		r.material = RenderMaterial;
		mf.mesh = MeshGenerator.GenerateTorus (0.05f,40, 0.01f, _lastSize/2);
		transform.localRotation = Quaternion.Euler(90,0,0);

		SetVisible (false);
	}

	void SetVisible(bool v)
	{
		_Visible = v;
		gameObject.GetComponent<MeshRenderer> ().enabled = v;
		
		if (DistanceLabel)
			DistanceLabel.enabled = v;
		if (SizeLabel)
			SizeLabel.enabled = v;
	}
	
	// Update is called once per frame
	void Update () {
		//Vector2 pos;

		if (_Visible) {
			transform.localPosition = new Vector3 (0, 0, Distance);

			_distancePos = ProjectPoint (Vector3.forward * 10);
			DistanceLabel.text = "距離:" + ToMetric (Distance);
			DistanceLabel.transform.localPosition = new Vector3 (_distancePos.x, _distancePos.y, 0);
		
			_sizePos = ProjectPoint (new Vector3 (_lastSize/2, 0, Distance));
			SizeLabel.text = "サイズ:" + ToMetric (Size);
			SizeLabel.transform.localPosition = new Vector3 (_sizePos.x, _sizePos.y, 0);

			if (Size != _lastSize) {
				MeshFilter mf = gameObject.GetComponent<MeshFilter> ();
				_lastSize = Size;
				MeshGenerator.ScaleTorus (mf.mesh, 0.01f, _lastSize * 0.01f, _lastSize/2, 40);
			}
			float dx=((Input.GetButton ("ScopeDistPos") ? 1 : 0) - (Input.GetButton ("ScopeDistNeg") ? 1 : 0));
			float dy=((Input.GetButton ("ScopeSizePos") ? 1 : 0) - (Input.GetButton ("ScopeSizeNeg") ? 1 : 0));
			Distance += dx * Time.deltaTime;
			Size += dy * Time.deltaTime;

			Distance = Mathf.Max (0.0f, Distance);
			Size = Mathf.Max (0.0f, Size);
		}
		if (Input.GetButtonDown ("ScopeSwitch")) {
			SetVisible(!_Visible);
		}
	}


	string ToMetric(float v)
	{
		float value = v;
		string unit = " m";
		if (v < 1) {
			value=v*100;
			unit=" cm";
		}else if (v > 1000) {
			value=v/1000.0f;
			unit=" km";
		}
		return value.ToString ("0.0") + unit;
	}
	/*
	void DrawCross(Vector2 pos,float size)
	{
		_CreateMaterial ();
		pos.x /= Screen.width;
		pos.y /= Screen.height;
		float sx=size / Screen.width;
		float sy=size / Screen.height;
		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadOrtho ();
		GL.Begin (GL.LINES);
		GL.Color (Color.red);
		GL.Vertex3 (pos.x - sx / 2, pos.y- sy / 2, 0);
		GL.Vertex3 (pos.x + sx / 2, pos.y+ sy / 2, 0);
		GL.End ();
		GL.Begin (GL.LINES);
		GL.Color (Color.red);
		GL.Vertex3 (pos.x + sx / 2, pos.y - sy / 2, 0);
		GL.Vertex3 (pos.x - sx / 2, pos.y + sy / 2, 0);
		GL.End ();
		GL.PopMatrix();
	}*/
	
	Rect _textArea = new Rect(0,0,Screen.width, Screen.height);

	Vector2 ProjectPoint(Vector3 pos)
	{
		Vector2 ret = TargetCamera.WorldToScreenPoint (TargetCamera.transform.position + TargetCamera.transform.rotation * pos);
		ret.x -= TargetCamera.pixelWidth / 2 + TargetCamera.pixelRect.xMin;
		ret.y -= TargetCamera.pixelHeight / 2  + TargetCamera.pixelRect.yMin;
		return ret;

	}
	Vector2 DrawString(string str,Vector3 pos,Color color,Vector2 Offset)
	{
		Camera c=TargetCamera;
		if (c == null)
			return Vector2.zero;

		Vector2 ret= ProjectPoint(pos);
		//DrawCross (_distancePos, 15);
		_textArea.position=ret+Offset;
		GUI.skin.GetStyle("Label").fontStyle=FontStyle.Bold;
		GUI.skin.GetStyle("Label").fontSize=20;
		GUI.skin.GetStyle("Label").normal.textColor=color;
		//textArea.position=_distancePos-textArea.size/2;
		
		_textArea.size= GUI.skin.GetStyle("Label").CalcSize(new GUIContent(str));
		GUI.Label(_textArea,str);
		return ret;
	}
	void DrawDistance()
	{
		string str="Distance:"+ToMetric(Distance);
		_distancePos= DrawString(str,Vector3.forward*10,Color.red,Vector2.zero);
	}

	void DrawSize()
	{
		string str="Size:"+ToMetric(Size);
		_sizePos= DrawString(str,new Vector3(Size/2,0,Distance),Color.green,new Vector2(0,15));
	}
	 void OnOVRGUI()
	{
	//	if (Camera.current != TargetCamera)
	//		return;
	//	_renderer.Begin (Camera.current.pixelRect.size);
		if(showText)
		{
			DrawDistance();
			DrawSize();
		}
		
	//	DrawCross (_distancePos, 15);
	//	DrawCross (_sizePos, 15);


	//	_renderer.End ();
	}
}
