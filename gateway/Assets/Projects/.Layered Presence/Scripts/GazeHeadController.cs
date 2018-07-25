using UnityEngine;
using System.Collections;

public class GazeHeadController : IRobotHeadControl {

	GazeFollowComponent _Gaze;
	public Vector2 Bounds=new Vector2(30,90.0f);
	Vector2 _angles=new Vector2();

	public Vector2 MinBounds=new Vector2(0,0.0f);
	public float Exp=1.2f;
	public bool UseMouse = false;

	MovingAverageF _posX = new MovingAverageF (10);
	MovingAverageF _posY = new MovingAverageF (10);
	float _X=0,_Y=0;


	public GazeHeadController()
	{
		_Gaze = GameObject.FindObjectOfType<GazeFollowComponent> ();
	}

	Vector2 GetPointGaze()
	{
		float a = Mathf.Sign(_Gaze.GazePointNormalized.x-0.5f)*Mathf.Pow (Mathf.Abs( _Gaze.GazePointNormalized.x-0.5f)*2, Exp);
		float b = Mathf.Sign(_Gaze.GazePointNormalized.y-0.5f)* Mathf.Pow (Mathf.Abs( _Gaze.GazePointNormalized.y-0.5f)*2, Exp);
		_X += (_posX.Add (a, 0.5f))*1*Time.deltaTime;
		_Y += (_posY.Add (b, 0.5f))*1.5f*Time.deltaTime;

		_X = Mathf.Clamp (_X, -1, 1);
		_Y = Mathf.Clamp (_Y, -1, 1);
		return new Vector2 (_X*0.5f+0.5f, _Y*0.5f+0.5f);

	}
	Vector2 GetPointMouse()
	{
		Vector2 v=new Vector2();
		v.x = (float)(Input.mousePosition.x/(float)Camera.main.pixelWidth) - 0.5f;
		v.y = (float)(Input.mousePosition.y/(float)Camera.main.pixelHeight) - 0.5f;

		float mx = MinBounds.x / (float)Camera.main.pixelWidth;
		float my = MinBounds.y / (float)Camera.main.pixelHeight;
		if (Mathf.Abs (v.x) - mx < 0)
			v.x = 0;
		else
			v.x = Mathf.Sign (v.x) * (Mathf.Abs (v.x) - mx);
		if (Mathf.Abs (v.y) - my < 0)
			v.y = 0;
		else
			v.y = Mathf.Sign (v.y) * (Mathf.Abs (v.y) - my);
		v.x = Mathf.Sign(v.x)*Mathf.Pow (Mathf.Abs( v.x*2), Exp)*0.5f+0.5f;
		v.y = Mathf.Sign(v.y)* Mathf.Pow (Mathf.Abs( v.y*2), Exp)*0.5f+0.5f;
		return v ;

	}

	bool isMousePressed=false;
	bool isOn=false;
	public bool GetHeadOrientation(out Quaternion q, bool abs) 
	{
		if (_Gaze == null) {
			q = Quaternion.identity;
			return false;
		}
		if (Input.GetMouseButtonDown (0) == true) {
			if (!isMousePressed)
				isOn = !isOn;
			isMousePressed = true;
		} else
			isMousePressed = false;
		if (isOn == false) {

			q = Quaternion.Euler (_angles.x, _angles.y, 0);
			return false;
		}
		Vector2 p;
		if (UseMouse)
			p = GetPointMouse ();
		else
			p = GetPointGaze ();
		float pitch = p.y-0.5f;
		float yaw = p.x-0.5f;
		pitch *= 2.0f;
		yaw *= 2.0f;

		pitch = Mathf.Clamp (pitch, -1, 1);
		yaw = Mathf.Clamp (yaw, -1, 1);
//		Debug.Log(pitch.ToString()+","+ yaw.ToString());


		_angles.x = (pitch) * Bounds.x ;
		_angles.y = -(yaw ) * Bounds.y ;
		
		_angles.x = Mathf.Clamp (_angles.x, -Bounds.x, Bounds.x);
		_angles.y = Mathf.Clamp (_angles.y, -Bounds.y, Bounds.y);

		Debug.Log (_angles.x.ToString () + "," + _angles.x.ToString ());
		q = Quaternion.Euler (_angles.x, _angles.y, 0);

		return true;
	}


	public bool GetHeadPosition(out Vector3 v,bool abs)
	{
		v = Vector3.zero;
		return false;
	}

	public void Recalibrate()
	{
		_angles = Vector2.zero;
	}
}
